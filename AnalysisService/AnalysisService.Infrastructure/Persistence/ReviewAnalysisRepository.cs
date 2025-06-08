using Marten;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure.Persistence;

internal sealed class ReviewAnalysisRepository(IDocumentSession session) : IReviewAnalysisRepository
{
    private readonly IDocumentSession session = session;

    public async Task InsertAsync(ReviewAnalysis entity, CancellationToken ct)
    {
        session.Store(entity);
        await session.SaveChangesAsync(ct);
    }

    public async Task<ReviewAnalysis?> FindByReviewIdAndStoreAsync(long reviewId, string store, CancellationToken ct)
    {
        return await session.Query<ReviewAnalysis>()
            .Where(r => r.ReviewId == reviewId && r.Store == store)
            .FirstOrDefaultAsync(ct);
    }
}