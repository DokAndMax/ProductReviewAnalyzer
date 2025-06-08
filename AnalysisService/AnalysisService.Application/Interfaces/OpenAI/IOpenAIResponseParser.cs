using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;

public interface IOpenAIResponseParser
{
    ReviewAnalysisResult Parse(string rawJson);
}
