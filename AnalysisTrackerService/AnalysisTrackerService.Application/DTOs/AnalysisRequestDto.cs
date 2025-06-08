using ProductReviewAnalyzer.AnalysisTrackerService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;

public record AnalysisRequestDto(
    Guid Id,
    Guid UserId,
    DateTime CreatedAtUtc,
    RequestStatus Status,
    string? DashboardUrl,
    IReadOnlyList<string> ProductUrls);