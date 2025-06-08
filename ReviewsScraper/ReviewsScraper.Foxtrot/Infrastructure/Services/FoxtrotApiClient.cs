using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;

public sealed class FoxtrotApiClient(
    IFoxtrotApi inner,
    IMemoryCache cache,
    ILogger<FoxtrotApiClient> logger)
    : IFoxtrotApi
{
    private static readonly MemoryCacheEntryOptions opts =
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

    public Task<string> GetReviewsAsync(
        long catalogObjectId,
        int classId,
        int page,
        int itemsPerPage,
        string brandTitle,
        CancellationToken ct = default)
    {
        var key = $"fx:{catalogObjectId}:{classId}:{page}";
        return cache.GetOrCreateAsync(key, entry =>
        {
            entry.SetOptions(opts);
            logger.LogDebug("Foxtrot Cache MISS for {Key}", key);
            return inner.GetReviewsAsync(catalogObjectId, classId, page, itemsPerPage, brandTitle, ct);
        })!;
    }
}