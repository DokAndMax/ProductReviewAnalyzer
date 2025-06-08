using ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Refit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFoxtrotInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddMemoryCache();

        var baseUrl = config["FoxtrotApi:BaseUrl"]
            ?? throw new InvalidOperationException("FoxtrotApi BaseUrl string not configured");

        services.AddHttpClient("foxtrot-page", c =>
            c.BaseAddress = new Uri(baseUrl));

        services.AddRefitClient<IFoxtrotApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
            );

        services.AddTransient<IFoxtrotApi>(sp =>
        {
            var realApi = RestService.For<IFoxtrotApi>(new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            });

            var cache = sp.GetRequiredService<IMemoryCache>();
            var log = sp.GetRequiredService<ILogger<FoxtrotApiClient>>();
            return new FoxtrotApiClient(realApi, cache, log);
        });

        services.AddSingleton<ProductPageParser>();
        services.AddSingleton<HtmlParser>();

        return services;
    }
}