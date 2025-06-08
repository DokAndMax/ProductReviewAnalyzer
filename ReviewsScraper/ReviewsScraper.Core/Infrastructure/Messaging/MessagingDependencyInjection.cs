using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.Interfaces;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Messaging;

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

        services.AddScoped<IReviewPublisher, ReviewPublisher>();

        return services;
    }
}