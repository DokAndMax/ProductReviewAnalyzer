    using ProductReviewAnalyzer.Contracts.Events;

    namespace ProductReviewAnalyzer.TableauExporterService.Features.AnalysisBuffer;

    public interface IAnalysisBuffer
    {
        void Add(ReviewAnalyzed message);
        IReadOnlyList<ReviewAnalyzed> Drain(Guid requestId);
    }