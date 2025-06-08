using ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;
using System.Linq.Expressions;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

public interface IAnalysisRepository
{
    Task AddAsync(AnalysisRequest entity, CancellationToken ct);
    Task<AnalysisRequest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<AnalysisRequest>> ListAsync(Expression<Func<AnalysisRequest, bool>>? filter, CancellationToken ct);
}