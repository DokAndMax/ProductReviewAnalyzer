using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.OpenAI;
using ProductReviewAnalyzer.AnalysisService.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.AnalysisService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddMartenStore(cfg);

        services.AddHttpClient<IOpenAIService, OpenAIService>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cfg["OpenAI:ApiKey"]);
        });

        services.AddScoped<IReviewAnalysisRepository, ReviewAnalysisRepository>();

        return services;
    }
}