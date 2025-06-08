using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Services;
using ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CreateAnalysisRequest;

public class CreateAnalysisRequestCommandHandler(
    IAnalysisRepository repo,
    IUnitOfWork uow,
    IUrlDispatchService dispatcher,
    IAnalysisBatchPublisher publisher) : IRequestHandler<CreateAnalysisRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateAnalysisRequestCommand cmd, CancellationToken ct)
    {
        var request = new AnalysisRequest
        {
            UserId = cmd.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            Items = cmd.ProductUrls.Select(u => new AnalysisItem { ProductUrl = u }).ToList()
        };

        await repo.AddAsync(request, ct);
        await uow.SaveChangesAsync(ct);

        var validUrls = cmd.ProductUrls
            .Where(url => ProductLinkParser.TryDetectStore(url, out _))
            .ToList();

        await publisher.PublishBatchStartedAsync(request.Id, validUrls.Count, ct);

        foreach (var url in validUrls)
        {
            ProductLinkParser.TryDetectStore(url, out var store);
            await dispatcher.DispatchAsync(request.Id, store, url, ct);
        }

        return request.Id;
    }
}