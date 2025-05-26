namespace ProductReviewAnalyzer.WebApp.Shared.Models;

public record HistoryRecordDto(Guid Id, string Url, DateTime RequestedAtUtc, int Status);