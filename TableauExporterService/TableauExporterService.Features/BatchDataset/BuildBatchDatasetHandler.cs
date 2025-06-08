using MediatR;
using ProductReviewAnalyzer.TableauExporterService.Features.Tableau;

namespace ProductReviewAnalyzer.TableauExporterService.Features.BatchDataset;

public sealed class BuildBatchDatasetHandler(IHyperFileBuilder hyperFileBuilder) : IRequestHandler<BuildBatchDatasetCommand, byte[]>
{
    private readonly IHyperFileBuilder hyperFileBuilder = hyperFileBuilder;

    public Task<byte[]> Handle(BuildBatchDatasetCommand request, CancellationToken cancellationToken)
    {
        var jsonDataset = BatchDatasetBuilder.Build(
            request.RequestId,
            request.ProductsId,
            request.Analyses);

        var hyperBytes = hyperFileBuilder.Build(jsonDataset);

        return Task.FromResult(hyperBytes);
    }
}
