namespace ProductReviewAnalyzer.Contracts.Events;

public record ProductAnalysisCompleted(
    Guid RequestId,
    int TotalProcessed,
    ICollection<long> ProductsId
    );