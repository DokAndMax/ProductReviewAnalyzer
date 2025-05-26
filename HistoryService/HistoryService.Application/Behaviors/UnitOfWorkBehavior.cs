using MediatR;
using ProductReviewAnalyzer.HistoryService.Application.Interfaces;

namespace ProductReviewAnalyzer.HistoryService.Application.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork uow) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(ct);
        await uow.SaveChangesAsync(ct);
        return response;
    }
}