using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;

public sealed class AlloApiClient(
    IAlloApi inner,
    IMemoryCache cache,
    ILogger<AlloApiClient> logger) : IAlloApi
{
    private readonly MemoryCacheEntryOptions opts = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

    public Task<AlloReviewsResponse> GetReviewsAsync(long productId, int page = 1, CancellationToken ct = default)
    {
        var key = $"allo:{productId}:p{page}";
        return cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetOptions(opts);
            logger.LogInformation("Allo Cache MISS for {Key}", key);
            return await inner.GetReviewsAsync(productId, page, ct);
        })!;
    }
}