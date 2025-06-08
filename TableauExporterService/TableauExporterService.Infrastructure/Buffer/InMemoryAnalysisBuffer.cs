using System.Collections.Concurrent;
using ProductReviewAnalyzer.Contracts.Events;
using ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Buffer;

public  sealed class InMemoryAnalysisBuffer : IAnalysisBuffer
{
    private readonly ConcurrentDictionary<Guid, ConcurrentBag<ReviewAnalyzed>> store = new();

    public void Add(ReviewAnalyzed a) =>
        store.GetOrAdd(a.RequestId, _ => []).Add(a);

    public IReadOnlyList<ReviewAnalyzed> Drain(Guid analysisId)
    {
        if (store.TryRemove(analysisId, out var bag))
            return [.. bag];
        return [];
    }
}