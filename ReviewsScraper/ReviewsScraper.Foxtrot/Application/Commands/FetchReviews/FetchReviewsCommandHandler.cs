using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandHandler(
    IHttpClientFactory httpFactory,
    ProductPageParser pageParser,
    IFoxtrotApi api,
    HtmlParser commentsParser,
    ReviewsDbContext db,
    IReviewPublisher publisher,
    IMapper mapper,
    ILogger<FetchReviewsCommandHandler> logger)
        : IRequestHandler<FetchReviewsCommand, int>
{
    private const int ItemsPerPage = 25;

    public async Task<int> Handle(FetchReviewsCommand request, CancellationToken ct)
    {
        var client = httpFactory.CreateClient("foxtrot-page");
        var productHtml = await client.GetStringAsync(request.ProductUrl, ct);
        var meta = pageParser.Parse(productHtml);

        if (meta is null)
            throw new ArgumentException($"catalogObjectId/classId не знайдено у сторінці {request.ProductUrl}");

        const string storeName = "Foxtrot";
        var store = await db.Stores.FirstOrDefaultAsync(s => s.Name == storeName, ct);
        if (store is null)
        {
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
            .FirstOrDefaultAsync(p => p.ExternalId == meta.CatalogObjectId && p.StoreId == store.Id, ct);

        if (product is null)
        {
            product = new Product
            {
                ExternalId = meta.CatalogObjectId,
                Title = meta.Title,
                StoreId = store.Id
            };
            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
        }
        else if (string.IsNullOrWhiteSpace(product.Title))
        {
            product.Title = meta.Title;
            db.Products.Update(product);
            await db.SaveChangesAsync(ct);
        }

        int page = 1, affected = 0;

        while (true)
        {
            var html = await api.GetReviewsAsync(
                meta.CatalogObjectId,
                meta.ClassId,
                page,
                ItemsPerPage,
                meta.BrandTitle,
                ct);

            var parsed = commentsParser.Parse(html).ToList();
            if (parsed.Count == 0) break;

            var reviewsToAdd = new List<Review>();
            var reviewsToUpdate = new List<Review>();

            foreach (var review in parsed)
            {
                var existing = await db.Reviews
                    .Include(r => r.Products)
                    .FirstOrDefaultAsync(r => r.ExternalId == review.ExternalId, ct);

                if (existing is not null)
                {
                    if (!existing.Products.Any(p => p.ExternalId == product.ExternalId && p.StoreId == store.Id))
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

            if (parsed.Count < ItemsPerPage) break;
            page++;
        }

        logger.LogInformation("Foxtrot: loaded/linked {Count} reviews for product {ProductId}",
            affected, product.ExternalId);

        await publisher.PublishScrapingCompletedAsync(request.RequestId, product.ExternalId, affected, ct);
        return affected;
    }
}
