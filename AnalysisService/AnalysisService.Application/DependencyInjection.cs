using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Application.Behaviors;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;
using ProductReviewAnalyzer.AnalysisService.Application.Mapping;
using ProductReviewAnalyzer.AnalysisService.Application.Services;

namespace ProductReviewAnalyzer.AnalysisService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IOpenAIService, OpenAIOrchestrator>();

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(RetryBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}