using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;

namespace ProductReviewAnalyzer.AnalysisService.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitConn = cfg.GetConnectionString("RabbitMQ")
                         ?? throw new InvalidOperationException("RabbitMQ connection string not configured");

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<NewReviewReceivedConsumer>();

            x.UsingRabbitMq((context, cfgRabbit) =>
            {
                cfgRabbit.Host(rabbitConn);

                cfgRabbit.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}