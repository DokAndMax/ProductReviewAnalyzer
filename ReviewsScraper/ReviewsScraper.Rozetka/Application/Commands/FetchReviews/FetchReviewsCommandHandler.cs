using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;
using System.Text.RegularExpressions;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;
using AutoMapper;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandHandler(
    IRozetkaApi api,
    HtmlParser parser,
    ReviewsDbContext db,
    IReviewPublisher publisher,
    IMapper mapper,
    ILogger<FetchReviewsCommandHandler> logger)
        : IRequestHandler<FetchReviewsCommand, int>
{
    private const int PageSize = 36;

    public async Task<int> Handle(FetchReviewsCommand request, CancellationToken ct)
    {
        if (!TryExtractProductId(request.ProductUrl, out var productId))
            throw new ArgumentException($"Не вдалося визначити productId з URL {request.ProductUrl}");

        const string storeName = "Rozetka";
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
            .FirstOrDefaultAsync(p => p.ExternalId == productId && p.StoreId == store.Id, ct);

        if (product is null)
        {
            product = new Product
            {
                ExternalId = productId,
                Title = string.Empty,
                StoreId = store.Id
            };
            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
        }

        int page = 1, affected = 0;

        while (true)
        {
            var resp = await api.GetReviewsAsync(productId, page, ct);
            var parsed = parser.Parse(resp).ToList();
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

            if (parsed.Count < PageSize) break;
            page++;
        }

        logger.LogInformation("Rozetka: loaded {Count} reviews for goods {ProductId}", affected, productId);
        await publisher.PublishScrapingCompletedAsync(request.RequestId, productId, affected, ct);
        return affected;
    }

    private static bool TryExtractProductId(string url, out long productId)
    {
        productId = 0;
        var m = Regex.Match(url, @"(?:/p|goods=)(\d+)(?:/|$)");
        return m.Success && long.TryParse(m.Groups[1].Value, out productId);
    }
}
