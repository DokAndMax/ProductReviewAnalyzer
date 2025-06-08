using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.TableauExporterService.Infrastructure.PubSub.Consumers;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.PubSub;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        // Отримуємо RabbitMQ connection string
        var rabbitConn = cfg.GetConnectionString("RabbitMQ")
            ?? throw new InvalidOperationException("RabbitMQ connection string not configured");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReviewAnalyzedConsumer>();
            x.AddConsumer<ProductAnalysisCompletedConsumer>();

            x.UsingRabbitMq((ctx, rabbit) =>
            {
                rabbit.Host(rabbitConn);

                rabbit.ReceiveEndpoint("tableau-exporter", e =>
                {
                    e.ConfigureConsumer<ReviewAnalyzedConsumer>(ctx);
                    e.ConfigureConsumer<ProductAnalysisCompletedConsumer>(ctx);

                    e.PrefetchCount = 16;
                    e.ConcurrentMessageLimit = 1;
                });
            });
        });

        return services;
    }
}
