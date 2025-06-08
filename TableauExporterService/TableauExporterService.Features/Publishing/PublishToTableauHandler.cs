using MediatR;
using ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing;

public sealed class PublishToTableauHandler(
    IDatasourcePublisher datasourcePublisher,
    IWorkbookPublisher workbookPublisher) : IRequestHandler<PublishToTableauCommand, string>
{
    private readonly IDatasourcePublisher datasourcePublisher = datasourcePublisher;
    private readonly IWorkbookPublisher workbookPublisher = workbookPublisher;

    public async Task<string> Handle(PublishToTableauCommand request, CancellationToken cancellationToken)
    {
        var contentUrl = await datasourcePublisher.PublishHyperAsync(
            projectName: "ProductReview",
            datasourceName: request.DatasourceName,
            hyperBytes: request.HyperBytes,
            cancellationToken: cancellationToken
        );

        var workbookName = $"Product_{request.RequestId}_Reviews";
        var dashboardUrl = await workbookPublisher.PublishWorkbookAsync(
            workbookName: workbookName,
            datasourceContentUrl: contentUrl,
            datasourceName: request.DatasourceName,
            overwrite: true,
            cancellationToken: cancellationToken
        );

        return dashboardUrl;
    }
}
