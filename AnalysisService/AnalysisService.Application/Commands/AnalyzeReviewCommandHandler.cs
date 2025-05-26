using MediatR;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisService.Application.Commands;

public sealed class AnalyzeReviewCommandHandler : IRequestHandler<AnalyzeReviewCommand, Guid>
{
    private readonly IOpenAIService _openAi;
    private readonly IReviewAnalysisRepository _repo;

    public AnalyzeReviewCommandHandler(IOpenAIService openAi, IReviewAnalysisRepository repo)
    {
        _openAi = openAi;
        _repo = repo;
    }

    public async Task<Guid> Handle(AnalyzeReviewCommand c, CancellationToken ct)
    {
        var (prodSent,
            storeSent,
            prodPros,
            prodCons,
            prodCats,
            prodUsage,
            storePros,
            storeCons) = await _openAi.AnalyzeAsync(c.Text, ct);

        var doc = new ReviewAnalysis
        {
            ReviewId = c.ReviewId,
            ProductId = c.ProductId,
            UserTitle = c.UserTitle,
            Mark = c.Mark,
            Dignity = c.Dignity,
            Shortcomings = c.Shortcomings,
            FromBuyer = c.FromBuyer,
            ReviewCreatedAt = c.CreatedAt,
            RawText = c.Text,

            ProductSentiment = prodSent,
            StoreSentiment = storeSent,
            ProductPros = prodPros,
            ProductCons = prodCons,
            ProductCategories = prodCats,
            ProductUsageInsights = prodUsage,
            StorePros = storePros,
            StoreCons = storeCons
        };

        await _repo.InsertAsync(doc, ct);
        return doc.Id;
    }
}