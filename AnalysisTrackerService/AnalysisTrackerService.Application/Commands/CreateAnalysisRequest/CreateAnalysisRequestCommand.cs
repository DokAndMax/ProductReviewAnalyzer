using MediatR;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CreateAnalysisRequest;

public record CreateAnalysisRequestCommand(Guid UserId, IReadOnlyList<string> ProductUrls) : IRequest<Guid>;