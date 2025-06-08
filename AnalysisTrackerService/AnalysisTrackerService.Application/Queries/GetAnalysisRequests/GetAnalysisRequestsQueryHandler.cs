using MediatR;
using MapsterMapper;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequests;

public class GetAnalysisRequestsQueryHandler(IAnalysisRepository repo, IMapper mapper) : IRequestHandler<GetAnalysisRequestsQuery, IReadOnlyList<AnalysisRequestDto>>
{
    private readonly IAnalysisRepository repo = repo;
    private readonly IMapper mapper = mapper;

    public async Task<IReadOnlyList<AnalysisRequestDto>> Handle(GetAnalysisRequestsQuery q, CancellationToken ct)
    {
        var entities = await repo.ListAsync(
            q.UserId is null
                ? null
                : r => r.UserId == q.UserId,
            ct);

        var dtos = entities
            .Select(r => mapper.Map<AnalysisRequestDto>(r))
            .OrderByDescending(d => d.CreatedAtUtc)
            .ToList();

        return dtos;
    }
}