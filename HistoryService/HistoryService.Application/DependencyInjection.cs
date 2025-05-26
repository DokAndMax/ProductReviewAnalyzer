using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.HistoryService.Application.Behaviors;

namespace ProductReviewAnalyzer.HistoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}