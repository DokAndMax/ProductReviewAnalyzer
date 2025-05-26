using MediatR;
namespace ProductReviewAnalyzer.AnalysisService.Application.Commands;

/// <summary>
/// Команда запускає LLM-аналіз конкретного відгуку
/// </summary>
public sealed record AnalyzeReviewCommand(
    long ReviewId,
    long ProductId,
    string UserTitle,
    int? Mark,
    string Text,
    string? Dignity,
    string? Shortcomings,
    bool FromBuyer,
    DateTime CreatedAt) : IRequest<Guid>;