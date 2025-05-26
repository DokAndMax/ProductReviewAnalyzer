using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Messaging;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;
using System.Text.RegularExpressions;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandHandler(
    IRozetkaApi api,
    HtmlParser parser,
    RozetkaDbContext db,
    IReviewPublisher publisher,
    ILogger<FetchReviewsCommandHandler> logger)
    : IRequestHandler<FetchReviewsCommand, int>
{
    private const int PageSize = 36;

    public async Task<int> Handle(FetchReviewsCommand request, CancellationToken ct)
    {
        if (!TryExtractProductId(request.ProductUrl, out var productId))
            throw new ArgumentException($"Не вдалося визначити productId з URL {request.ProductUrl}");

        int page = 1, added = 0;
        while (true)
        {
            var apiResp = await api.GetCommentsAsync(productId, page, ct);
            var reviews = parser.Parse(apiResp).ToList();
            if (reviews.Count == 0) break;

            foreach (var r in reviews)
            {
                if (await db.Reviews.AsNoTracking()
                        .AnyAsync(x => x.ExternalId == r.ExternalId, ct))
                    continue;

                db.Reviews.Add(r);
                await publisher.PublishAsync(r, ct);
                added++;
            }

            await db.SaveChangesAsync(ct);

            if (reviews.Count < PageSize) break;
            page++;
        }

        logger.LogInformation("Loaded {Count} new reviews for product {ProductId}", added, productId);
        return added;
    }

    private static bool TryExtractProductId(string url, out long productId)
    {
        productId = 0;
        var match = Regex.Match(url, @"(?:p|goods=)(\d+)");
        return match.Success && long.TryParse(match.Groups[1].Value, out productId);
    }
}
