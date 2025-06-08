using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRozetkaApplication(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));
        services.AddValidatorsFromAssembly(asm);

        return services;
    }
}