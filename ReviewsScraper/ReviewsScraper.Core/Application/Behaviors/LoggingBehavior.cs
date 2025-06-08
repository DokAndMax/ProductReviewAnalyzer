using MediatR;
using Microsoft.Extensions.Logging;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        logger.LogDebug("Handling {Request}", typeof(TRequest).Name);
        var response = await next(ct);
        logger.LogDebug("Handled {Request}", typeof(TRequest).Name);
        return response;
    }
}