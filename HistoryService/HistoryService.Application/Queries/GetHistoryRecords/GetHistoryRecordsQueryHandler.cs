using MediatR;
using ProductReviewAnalyzer.HistoryService.Application.DTOs;
using ProductReviewAnalyzer.HistoryService.Application.Interfaces;

namespace ProductReviewAnalyzer.HistoryService.Application.Queries.GetHistoryRecords;

public class GetHistoryRecordsQueryHandler(IHistoryRepository repo)
    : IRequestHandler<GetHistoryRecordsQuery, IReadOnlyList<HistoryRecordDto>>
{
    public async Task<IReadOnlyList<HistoryRecordDto>> Handle(GetHistoryRecordsQuery _, CancellationToken ct)
    {
        var list = await repo.ListAsync(null, ct);

        return list.Select(e => new HistoryRecordDto(e.Id, e.Url, e.RequestedAtUtc, e.Status))
            .OrderByDescending(d => d.RequestedAtUtc)
            .ToList();
    }
}