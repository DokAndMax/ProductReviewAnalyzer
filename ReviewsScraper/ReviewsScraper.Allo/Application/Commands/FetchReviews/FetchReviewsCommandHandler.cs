using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandHandler(
    IAlloApi api,
    IAlloHtmlParser htmlParser,
    IAlloReviewsParser reviewsParser,
    IHttpClientFactory httpFactory,
    ReviewsDbContext db,
    IReviewPublisher publisher,
    IMapper mapper,
    ILogger<FetchReviewsCommandHandler> logger)
        : IRequestHandler<FetchReviewsCommand, int>
{
    private const int PageSize = 8;

    public async Task<int> Handle(FetchReviewsCommand request, CancellationToken ct)
    {
        logger.LogInformation("Handler started: RequestId={RequestId}, ProductUrl={ProductUrl}",
            request.RequestId, request.ProductUrl);

        var http = httpFactory.CreateClient("allo-html");
        var html = await http.GetStringAsync(request.ProductUrl, ct);

        if (!htmlParser.TryExtractMeta(html, out var productId, out var productTitle))
        {
            logger.LogError("Failed to extract product metadata: Url={ProductUrl}", request.ProductUrl);
            throw new ArgumentException($"Unable to extract productId from URL {request.ProductUrl}");
        }
        logger.LogInformation("Extracted metadata: ProductId={ProductId}, Title=\"{Title}\"",
            productId, productTitle);

        const string storeName = "Allo";

        var store = await db.Stores.FirstOrDefaultAsync(s => s.Name == storeName, ct);
        if (store is null)
        {
            logger.LogInformation("Store not found, creating new store record: StoreName={StoreName}", storeName);
            store = new Store { Name = storeName };
            db.Stores.Add(store);
            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            when (ex.InnerException?.Message.Contains("duplicate") == true
            || ex.InnerException?.Message.Contains("IX_Stores_Name") == true)
            {
                db.Entry(store).State = EntityState.Detached;
                store = await db.Stores.FirstOrDefaultAsync(s => s.Name == storeName, ct);
            }
        }

        var product = await db.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.ExternalId == productId && p.StoreId == store.Id, ct);

        if (product is null)
        {
            product = new Product
            {
                ExternalId = productId,
                Title = productTitle,
                StoreId = store.Id
            };
            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
        }
        else if (string.IsNullOrWhiteSpace(product.Title))
        {
            product.Title = productTitle;
            db.Products.Update(product);
            await db.SaveChangesAsync(ct);
        }

        int page = 1, affected = 0;
        AlloReviewsResponse resp;
        do
        {
            logger.LogInformation("Fetching reviews page: ProductId={ProductId}, Page={Page}",
                productId, page);

            resp = await api.GetReviewsAsync(productId, page, ct);
            var parsed = reviewsParser.Parse(resp).ToList();

            logger.LogDebug("Parsed {Count} items from API response", parsed.Count);

            var reviewsToAdd = new List<Review>();
            var reviewsToUpdate = new List<Review>();

            foreach (var review in parsed)
            {
                var existing = await db.Reviews
                    .Include(r => r.Products)
                    .FirstOrDefaultAsync(r => r.ExternalId == review.ExternalId, ct);

                if (existing is not null)
                {
                    if (!existing.Products.Any(p => p.ExternalId == productId && p.StoreId == store.Id))
                    {
                        existing.Products.Add(product);
                        reviewsToUpdate.Add(existing);
                    }

                    var reviewScrapedData = mapper.Map<ReviewScrapedData>((existing, product));
                    await publisher.PublishAsync(request.RequestId, reviewScrapedData, ct);
                    affected++;
                }
                else
                {
                    review.Products.Add(product);
                    reviewsToAdd.Add(review);

                    var reviewScrapedData = mapper.Map<ReviewScrapedData>((review, product));
                    await publisher.PublishAsync(request.RequestId, reviewScrapedData, ct);

                    affected++;
                }
            }

            if (reviewsToUpdate.Any())
                db.Reviews.UpdateRange(reviewsToUpdate);

            if (reviewsToAdd.Any())
                db.Reviews.AddRange(reviewsToAdd);

            await db.SaveChangesAsync(ct);

            page++;
        }
        while (resp.items.Length == PageSize);

        logger.LogInformation("Allo: loaded {Count} reviews for goods {ProductId}", affected, productId);
        await publisher.PublishScrapingCompletedAsync(request.RequestId, productId, affected, ct);
        return affected;
    }
}