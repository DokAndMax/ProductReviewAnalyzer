using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.HistoryService.Application.Interfaces;
using ProductReviewAnalyzer.HistoryService.Domain.Entities;
using System.Linq.Expressions;

namespace ProductReviewAnalyzer.HistoryService.Infrastructure.Persistence;

internal sealed class HistoryRepository(HistoryDbContext db) : IHistoryRepository
{
    public async Task AddAsync(HistoryRecord entity, CancellationToken ct)
    {
        await db.HistoryRecords.AddAsync(entity, ct);
    }

    public async Task<IReadOnlyList<HistoryRecord>> ListAsync(Expression<Func<HistoryRecord, bool>>? filter, CancellationToken ct)
    {
        IQueryable<HistoryRecord> query = db.HistoryRecords.AsNoTracking();
        if (filter is not null)
            query = query.Where(filter);
        return await query.ToListAsync(ct);
    }
}