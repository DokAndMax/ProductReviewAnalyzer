using MediatR;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisService.Domain.Events;

public record ReviewAnalyzedDomainEvent(ReviewAnalysis Analysis) : INotification;