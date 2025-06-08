using MediatR;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer.DrainBuffer;

public sealed record DrainBufferCommand(Guid RequestId)
    : IRequest<IReadOnlyList<ReviewAnalyzed>>;