namespace ProductReviewAnalyzer.Common.Configuration;

public sealed record OpenAIOptions
{
    public const string SectionName = "OpenAI";
    public required string ApiKey { get; init; }
    public string? Organization { get; init; }
}