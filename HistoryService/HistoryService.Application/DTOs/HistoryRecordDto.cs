using ProductReviewAnalyzer.HistoryService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.HistoryService.Application.DTOs;

public record HistoryRecordDto(Guid Id, string Url, DateTime RequestedAtUtc, RequestStatus Status);