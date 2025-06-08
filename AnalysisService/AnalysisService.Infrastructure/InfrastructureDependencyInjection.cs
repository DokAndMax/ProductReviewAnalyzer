using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI.Http;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI.Parsing;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI.Prompt;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddSingleton<IOpenAIPromptBuilder, OpenAIPromptBuilder>();
        services.AddSingleton<IOpenAIClient, OpenAIClient>();
        services.AddSingleton<IOpenAIResponseParser, OpenAIResponseParser>();

        services.AddMartenStore(cfg);

        var baseUrl = cfg["OpenAI:BaseUrl"]
            ?? throw new ArgumentException("OpenAI:ApiKey is not configured");

        services.AddHttpClient<IOpenAIClient, OpenAIClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddScoped<IReviewAnalysisRepository, ReviewAnalysisRepository>();

        return services;
    }
}