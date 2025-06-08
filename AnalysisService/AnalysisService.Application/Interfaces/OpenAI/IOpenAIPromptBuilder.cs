namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;

public interface IOpenAIPromptBuilder
{
    string BuildPayload(string reviewText);
}
