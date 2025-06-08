using MediatR;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CompleteAnalysisRequest;

public record CompleteAnalysisRequestCommand(Guid RequestId, string DashboardUrl, bool IsSuccess) : IRequest<Unit>;