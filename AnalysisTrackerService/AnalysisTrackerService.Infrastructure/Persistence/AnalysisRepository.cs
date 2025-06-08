using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;
using System.Linq.Expressions;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure.Persistence;

internal sealed class AnalysisRepository(AnalysisDbContext db) : IAnalysisRepository
{
    public async Task AddAsync(AnalysisRequest entity, CancellationToken ct) => await db.AnalysisRequests.AddAsync(entity, ct);

    public async Task<AnalysisRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.AnalysisRequests.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<AnalysisRequest>> ListAsync(Expression<Func<AnalysisRequest, bool>>? filter, CancellationToken ct)
    {
        IQueryable<AnalysisRequest> q = db.AnalysisRequests.Include(r => r.Items).AsNoTracking();
        if (filter is not null) q = q.Where(filter);
        return await q.ToListAsync(ct);
    }
}