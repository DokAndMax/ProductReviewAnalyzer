using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public sealed class RozetkaApiClient(
    IRozetkaApi inner,
    IMemoryCache cache,
    ILogger<RozetkaApiClient> logger)
    : IRozetkaApi
{
    private readonly MemoryCacheEntryOptions opts = new()
        { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

    public Task<RozetkaCommentsResponse> GetReviewsAsync(
        long goods, int page = 1, CancellationToken ct = default)
    {
        var key = $"rz:{goods}:p{page}";
        return cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetOptions(opts);
            logger.LogInformation("Rozetka Cache MISS for {Key}", key);
            return await inner.GetReviewsAsync(goods, page, ct);
        })!;
    }
}