using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ProductReviewAnalyzer.Common.Configuration;

namespace ProductReviewAnalyzer.Common.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCommon(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<OpenAIOptions>(config.GetSection(OpenAIOptions.SectionName));
        return services;
    }
}