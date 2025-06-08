using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequests;

public record GetAnalysisRequestsQuery(Guid? UserId) : IRequest<IReadOnlyList<AnalysisRequestDto>>;