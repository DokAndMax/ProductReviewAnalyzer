using Microsoft.Extensions.Caching.Memory;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public sealed class RozetkaApiClient(IRozetkaApi inner, IMemoryCache cache, ILogger<RozetkaApiClient> logger)
    : IRozetkaApi
{
    private readonly MemoryCacheEntryOptions _opts = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public Task<RozetkaCommentsResponse> GetCommentsAsync(long goods, int page = 1, CancellationToken ct = default)
    {
        var key = $"rz-{goods}-p{page}";
        return cache.GetOrCreateAsync(key, async _ =>
        {
            logger.LogInformation("Cache MISS for {Key}", key);
            return await inner.GetCommentsAsync(goods, page, ct);
        })!;
    }
}