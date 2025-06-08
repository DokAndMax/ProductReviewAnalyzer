using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Messaging.Consumers;
using ProductReviewAnalyzer.AnalysisService.Messaging.Mapping;
using ProductReviewAnalyzer.AnalysisService.Messaging.Publishers;
using ProductReviewAnalyzer.AnalysisService.Messaging.Sagas;
using RabbitMQ.Client;

namespace ProductReviewAnalyzer.AnalysisService.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        var rabbitConn = cfg.GetConnectionString("RabbitMQ")
                         ?? throw new InvalidOperationException("RabbitMQ connection string not configured");

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<ReviewScrapedConsumer>();

            x.AddSagaStateMachine<ProductReviewStateMachine, ProductReviewSaga>()
                .InMemoryRepository();

            x.UsingRabbitMq((context, cfgRabbit) =>
            {
                cfgRabbit.Host(rabbitConn);

                cfgRabbit.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IReviewAnalyzedPublisher, ReviewAnalyzedPublisher>();

        return services;
    }

    public static IHealthChecksBuilder AddRabbitMQHealthChecks(this IHealthChecksBuilder healthChecks, IConfiguration cfg)
    {
        var rabbitConn = cfg.GetConnectionString("RabbitMQ")
                         ?? throw new InvalidOperationException("RabbitMQ connection string not configured");

        healthChecks.AddRabbitMQ(sp =>
        {
            var rabbitUri = cfg.GetConnectionString("RabbitMQ")
                            ?? throw new InvalidOperationException("RabbitMQ connection string not configured");
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitUri)
            };
            return factory.CreateConnectionAsync();
        }, name: "rabbitmq");

        return healthChecks;
    }
}