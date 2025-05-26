using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Application.Behaviors;

namespace ProductReviewAnalyzer.AnalysisService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(RetryBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}