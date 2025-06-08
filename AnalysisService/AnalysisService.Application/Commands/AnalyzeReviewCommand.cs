using MediatR;
namespace ProductReviewAnalyzer.AnalysisService.Application.Commands;

public sealed record AnalyzeReviewCommand(
    Guid RequestId,
    long ReviewId,
    long ProductId,
    string ProductTitle,
    string Store,
    string UserTitle,
    int? Mark,
    string Text,
    string? Dignity,
    string? Shortcomings,
    bool FromBuyer,
    DateTime CreatedAt) : IRequest<Guid>;