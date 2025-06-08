namespace ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;

public interface IOpenAIClient
{
    Task<string> GetRawResponseAsync(string jsonRequestPayload, CancellationToken cancellationToken = default);
}
