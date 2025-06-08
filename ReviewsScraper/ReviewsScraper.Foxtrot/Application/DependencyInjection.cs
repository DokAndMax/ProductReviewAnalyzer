using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFoxtrotApplication(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));
        services.AddValidatorsFromAssembly(asm);

        return services;
    }
}