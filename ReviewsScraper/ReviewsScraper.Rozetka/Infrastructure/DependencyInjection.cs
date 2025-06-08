using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;
using Refit;
using System.Net;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRozetkaInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddMemoryCache();

        var baseUrl = config["RozetkaApi:BaseUrl"]
            ?? throw new InvalidOperationException("RozetkaApi BaseUrl string not configured");

        services.AddRefitClient<IRozetkaApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
            );

        services.AddTransient<IRozetkaApi>(sp =>
        {
            var realApi = RestService.For<IRozetkaApi>(new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            });

            var cache = sp.GetRequiredService<IMemoryCache>();
            var log = sp.GetRequiredService<ILogger<RozetkaApiClient>>();
            return new RozetkaApiClient(realApi, cache, log);
        });

        services.AddSingleton<HtmlParser>();

        return services;
    }
}