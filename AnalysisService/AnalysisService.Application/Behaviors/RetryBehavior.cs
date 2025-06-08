using MediatR;
using Polly;
using Polly.Retry;

namespace ProductReviewAnalyzer.AnalysisService.Application.Behaviors;

public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly AsyncRetryPolicy policy =
        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        return await policy.ExecuteAsync(_ => next(), ct);
    }
}