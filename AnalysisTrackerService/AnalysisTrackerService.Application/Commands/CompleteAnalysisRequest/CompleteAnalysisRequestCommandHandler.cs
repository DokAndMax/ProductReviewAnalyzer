using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CompleteAnalysisRequest;

public class CompleteAnalysisRequestCommandHandler(
    IAnalysisRepository repo,
    IUnitOfWork uow,
    IAnalysisNotifier notifier) : IRequestHandler<CompleteAnalysisRequestCommand, Unit>
{
    public async Task<Unit> Handle(CompleteAnalysisRequestCommand cmd, CancellationToken ct)
    {
        var request = await repo.GetByIdAsync(cmd.RequestId, ct)
                      ?? throw new InvalidOperationException($"Request {cmd.RequestId} not found");

        if (cmd.IsSuccess) request.MarkCompleted(cmd.DashboardUrl);
        else request.MarkFailed();

        await uow.SaveChangesAsync(ct);

        await notifier.NotifyCompletedAsync(request.UserId,
            request.Id,
            request.DashboardUrl,
            (int)request.Status,
            ct);

        return Unit.Value;
    }
}