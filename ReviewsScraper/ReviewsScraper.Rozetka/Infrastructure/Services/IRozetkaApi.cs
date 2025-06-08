using Refit;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public interface IRozetkaApi
{
    [Get("/v4/comments/get?country=UA&lang=ua&type=comment&limit=36&sort=from_buyer&topSellerId=5&goods={goods}&page={page}")]
    Task<RozetkaCommentsResponse> GetReviewsAsync(
        [AliasAs("goods")] long goods,
        [AliasAs("page")] int page = 1,
        CancellationToken ct = default);
}