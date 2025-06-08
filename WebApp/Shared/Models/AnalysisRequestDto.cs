using ProductReviewAnalyzer.AnalysisTrackerService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.WebApp.Shared.Models;

public record AnalysisRequestDto(
    Guid Id,
    Guid UserId,
    DateTime CreatedAtUtc,
    RequestStatus Status,
    string? DashboardUrl,
    IReadOnlyList<string> ProductUrls);