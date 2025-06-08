using MapsterMapper;
using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequestById;

public sealed class GetAnalysisRequestByIdQueryHandler(
    IAnalysisRepository repo,
    IMapper mapper)
    : IRequestHandler<GetAnalysisRequestByIdQuery, AnalysisRequestDto?>
{
    public async Task<AnalysisRequestDto?> Handle(
        GetAnalysisRequestByIdQuery q,
        CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(q.RequestId, ct);
        return entity is null ? null : mapper.Map<AnalysisRequestDto>(entity);
    }
}