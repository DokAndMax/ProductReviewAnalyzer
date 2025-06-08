using MediatR;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer.DrainBuffer;

internal sealed class DrainBufferHandler(IAnalysisBuffer buffer)
    : IRequestHandler<DrainBufferCommand, IReadOnlyList<ReviewAnalyzed>>
{
    public Task<IReadOnlyList<ReviewAnalyzed>> Handle(
        DrainBufferCommand cmd, CancellationToken _)
        => Task.FromResult(buffer.Drain(cmd.RequestId));
}