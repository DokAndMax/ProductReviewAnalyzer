using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisTrackerService.Messaging.Publishers;
using RabbitMQ.Client;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitConn = cfg.GetConnectionString("RabbitMQ")
                         ?? throw new InvalidOperationException("RabbitMQ connection string not configured");

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfgRabbit) =>
            {
                cfgRabbit.Host(rabbitConn);

                cfgRabbit.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IAnalysisBatchPublisher, AnalysisBatchPublisher>();

        return services;
    }
}