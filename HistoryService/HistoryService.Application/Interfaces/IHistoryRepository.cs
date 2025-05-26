using ProductReviewAnalyzer.HistoryService.Domain.Entities;
using System.Linq.Expressions;

namespace ProductReviewAnalyzer.HistoryService.Application.Interfaces;

public interface IHistoryRepository
{
    Task AddAsync(HistoryRecord entity, CancellationToken ct);
    Task<IReadOnlyList<HistoryRecord>> ListAsync(Expression<Func<HistoryRecord, bool>>? filter, CancellationToken ct);
}