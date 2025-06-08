using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAlloApplication(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));
        services.AddValidatorsFromAssembly(asm);

        return services;
    }
}