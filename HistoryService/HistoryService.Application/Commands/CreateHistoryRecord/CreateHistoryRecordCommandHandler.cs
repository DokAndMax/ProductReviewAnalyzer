using MediatR;
using ProductReviewAnalyzer.HistoryService.Application.Interfaces;
using ProductReviewAnalyzer.HistoryService.Domain.Entities;

namespace ProductReviewAnalyzer.HistoryService.Application.Commands.CreateHistoryRecord;

public class CreateHistoryRecordCommandHandler(IHistoryRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateHistoryRecordCommand, Guid>
{
    public async Task<Guid> Handle(CreateHistoryRecordCommand cmd, CancellationToken ct)
    {
        var entity = new HistoryRecord
        {
            Url = cmd.Url,
            RequestedAtUtc = DateTime.UtcNow,
            Status = cmd.Status
        };

        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        return entity.Id;
    }
}