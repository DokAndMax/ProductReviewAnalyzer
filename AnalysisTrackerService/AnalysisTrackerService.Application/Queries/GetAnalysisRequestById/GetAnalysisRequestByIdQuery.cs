using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequestById;

public sealed record GetAnalysisRequestByIdQuery(Guid RequestId)
    : IRequest<AnalysisRequestDto?>;