using MediatR;
using ProductReviewAnalyzer.HistoryService.Application.DTOs;

namespace ProductReviewAnalyzer.HistoryService.Application.Queries.GetHistoryRecords;

public record GetHistoryRecordsQuery : IRequest<IReadOnlyList<HistoryRecordDto>>;

