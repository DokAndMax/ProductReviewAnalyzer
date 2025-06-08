using MassTransit;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Sagas;

public class ProductReviewStateMachine :
    MassTransitStateMachine<ProductReviewSaga>
{
    public State Buffered { get; private set; } = default!;
    public State Processing { get; private set; } = default!;

    public Event<AnalysisBatchStarted> AnalysisBatchStarted { get; private set; } = default!;
    public Event<ScrapingCompleted> ScrapingCompleted { get; private set; } = default!;
    public Event<ReviewAnalyzed> ReviewAnalyzed { get; private set; } = default!;

    public ProductReviewStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => AnalysisBatchStarted, x =>
        {
            x.CorrelateById(x => x.Message.RequestId); 

            x.InsertOnInitial = true;

            x.SetSagaFactory(ctx =>
            {
                return new ProductReviewSaga
                {
                    CorrelationId = ctx.Message.RequestId,
                    ExpectedReviewsCount = 0,
                    ExpectedProductsCount = ctx.Message.TotalProducts,
                    ReceivedReviewsCount = 0,
                    ProductsId = [],
                };
            });
        });

        Event(() => ScrapingCompleted, x =>
        {
            x.CorrelateById(x => x.Message.RequestId);

            x.InsertOnInitial = true;

            x.SetSagaFactory(ctx =>
            {
                return new ProductReviewSaga
                {
                    CorrelationId = ctx.Message.RequestId,
                    ExpectedReviewsCount = ctx.Message.TotalComments,
                    ExpectedProductsCount = 0,
                    ReceivedReviewsCount = 0,
                    ProductsId = [ctx.Message.ProductId],
                };
            });
        });

        Event(() => ReviewAnalyzed, 
            x =>
            {
                x.CorrelateById(x => x.Message.RequestId);

                x.InsertOnInitial = true;
                x.SetSagaFactory(ctx =>
                {
                    return new ProductReviewSaga
                    {
                        CorrelationId = ctx.Message.RequestId,
                        ExpectedReviewsCount = 0,
                        ExpectedProductsCount = 0,
                        ReceivedReviewsCount = 1,
                        ProductsId = [ctx.Message.ProductId],
                    };
                });
            });

        Initially(
            When(ReviewAnalyzed)
                .TransitionTo(Buffered)
        );

        Initially(
            When(ScrapingCompleted)
                .TransitionTo(Buffered)
        );

        Initially(
            When(AnalysisBatchStarted)
                .TransitionTo(Processing)
        );

        During(Buffered,
            When(ReviewAnalyzed)
                .Then(ctx =>
                {
                    ctx.Saga.IncrementReceivedReviewsCount();
                }),

            When(ScrapingCompleted)
                .Then(ctx =>
                {
                    ctx.Saga.AddExpectedReviewsCount(ctx.Message.TotalComments);

                    ctx.Saga.ProductsId.Add(ctx.Message.ProductId);
                }),

            When(AnalysisBatchStarted)
                .Then(ctx =>
                {
                    ctx.Saga.ExpectedProductsCount = ctx.Message.TotalProducts;
                })
                .If(ctx => ctx.Saga.ReceivedReviewsCount >= ctx.Saga.ExpectedReviewsCount
                           && ctx.Saga.ProductsId.Count >= ctx.Saga.ExpectedProductsCount,
                    binder => binder
                    .Publish(ctx => new ProductAnalysisCompleted(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.ReceivedReviewsCount,
                        ctx.Saga.ProductsId)))
                .Finalize()
                .TransitionTo(Processing)
        );


        During(Processing,
            When(ReviewAnalyzed)
                .Then(ctx =>
                {
                    ctx.Saga.IncrementReceivedReviewsCount();
                })
                .If(ctx => ctx.Saga.ReceivedReviewsCount >= ctx.Saga.ExpectedReviewsCount 
                           && ctx.Saga.ProductsId.Count >= ctx.Saga.ExpectedProductsCount,
                    binder => binder
                    .Publish(ctx => new ProductAnalysisCompleted(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.ReceivedReviewsCount,
                        ctx.Saga.ProductsId))
                    .Finalize()
                ),

            When(ScrapingCompleted)
                .Then(ctx =>
                {
                    ctx.Saga.AddExpectedReviewsCount(ctx.Message.TotalComments);
                    ctx.Saga.ProductsId.Add(ctx.Message.ProductId);
                })
                .If(ctx => ctx.Saga.ReceivedReviewsCount >= ctx.Saga.ExpectedReviewsCount
                           && ctx.Saga.ProductsId.Count >= ctx.Saga.ExpectedProductsCount,
                    binder => binder
                    .Publish(ctx => new ProductAnalysisCompleted(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.ReceivedReviewsCount,
                        ctx.Saga.ProductsId))
                    .Finalize()
                )
        );

        SetCompletedWhenFinalized();
    }
}