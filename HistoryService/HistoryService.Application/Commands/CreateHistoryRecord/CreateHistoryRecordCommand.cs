using MediatR;
using ProductReviewAnalyzer.HistoryService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.HistoryService.Application.Commands.CreateHistoryRecord;

public record CreateHistoryRecordCommand(string Url, RequestStatus Status) : IRequest<Guid>;