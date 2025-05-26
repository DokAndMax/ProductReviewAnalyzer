using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductReviewAnalyzer.Common.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Behaviors;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Messaging;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;
using Refit;
using System.Net;
using System.Reflection;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var asm = Assembly.GetExecutingAssembly();

        var conn = config.GetConnectionString("SqlServer")
                   ?? throw new InvalidOperationException("SqlServer connection string not configured");

        services.AddDbContextWithMigrator<RozetkaDbContext>(opt =>
            opt.UseSqlServer(conn));

        services.AddRefitClient<IRozetkaApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(config["RozetkaApi:BaseUrl"]!);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
            );

        services.AddMemoryCache();
        services.AddTransient<IRozetkaApi>(sp =>
        {
            var inner = RestService.For<IRozetkaApi>(new HttpClient
            {
                BaseAddress = new Uri(config["RozetkaApi:BaseUrl"]!)
            });
            var cache = sp.GetRequiredService<IMemoryCache>();
            var log = sp.GetRequiredService<ILogger<RozetkaApiClient>>();
            return new RozetkaApiClient(inner, cache, log);
        });

        services.AddSingleton<HtmlParser>();

        services.AddScoped<IReviewPublisher, ReviewPublisher>();

        services.AddAutoMapper(asm);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}