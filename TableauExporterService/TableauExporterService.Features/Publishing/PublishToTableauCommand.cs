using MediatR;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing;

public sealed record PublishToTableauCommand(
    Guid RequestId,
    byte[] HyperBytes,
    string DatasourceName
) : IRequest<string>;