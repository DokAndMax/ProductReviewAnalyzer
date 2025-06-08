using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;
using Refit;
using System.Net;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAlloInfrastructure(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddMemoryCache();

        var baseUrl = cfg["AlloApi:BaseUrl"]
            ?? throw new InvalidOperationException("AlloApi BaseUrl string not configured");

        services.AddRefitClient<IAlloApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }
            );

        services.AddTransient<IAlloApi>(sp =>
        {
            var realApi = RestService.For<IAlloApi>(new HttpClient 
            {
                BaseAddress = new Uri(baseUrl)
            });

            var cache = sp.GetRequiredService<IMemoryCache>();
            var log = sp.GetRequiredService<ILogger<AlloApiClient>>();
            return new AlloApiClient(realApi, cache, log);
        });

        services.AddHttpClient("allo-html", c => c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0"));

        services.AddSingleton<IAlloHtmlParser, AlloHtmlParser>();
        services.AddSingleton<IAlloReviewsParser, AlloReviewsParser>();

        return services;
    }
}