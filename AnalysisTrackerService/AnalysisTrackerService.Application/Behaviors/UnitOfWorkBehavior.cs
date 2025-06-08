using MediatR;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Behaviors;

public class UnitOfWorkBehavior<TReq, TRes>(IUnitOfWork uow)
    : IPipelineBehavior<TReq, TRes> where TReq : notnull
{
    public async Task<TRes> Handle(TReq req, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var res = await next();
        await uow.SaveChangesAsync(ct);
        return res;
    }
}