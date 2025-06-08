using MediatR;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Logging;

internal sealed class CreateLogEntryHandler(ILogRepository repo)
    : IRequestHandler<CreateLogEntryCommand>
{
    public Task Handle(CreateLogEntryCommand cmd, CancellationToken ct)
        => repo.InsertAsync(cmd.Entry, ct);
}
