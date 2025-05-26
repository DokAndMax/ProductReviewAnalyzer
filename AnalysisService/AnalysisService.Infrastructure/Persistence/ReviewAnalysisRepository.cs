using Marten;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.Persistence;

internal sealed class ReviewAnalysisRepository : IReviewAnalysisRepository
{
    private readonly IDocumentSession _session;
    public ReviewAnalysisRepository(IDocumentSession session) => _session = session;

    public async Task InsertAsync(ReviewAnalysis entity, CancellationToken ct)
    {
        _session.Store(entity);
        await _session.SaveChangesAsync(ct);
    }
}