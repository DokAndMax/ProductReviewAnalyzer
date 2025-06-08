using MediatR;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.TableauExporterService.Features.BatchDataset;

public sealed record BuildBatchDatasetCommand(
    Guid RequestId,
    ICollection<long> ProductsId,
    IReadOnlyList<ReviewAnalyzed> Analyses
) : IRequest<byte[]>;