using MediatR;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;
using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;
using System.Text;
using ProductReviewAnalyzer.AnalysisService.Application.DTOs;
using AutoMapper;
using ProductReviewAnalyzer.AnalysisService.Application.Interfaces.OpenAI;

namespace ProductReviewAnalyzer.AnalysisService.Application.Commands;

public sealed class AnalyzeReviewCommandHandler(
    IOpenAIService openAi,
    IReviewAnalysisRepository repo,
    IReviewAnalyzedPublisher publisher,
    IMapper mapper,
    ILogger<AnalyzeReviewCommandHandler> log)
    : IRequestHandler<AnalyzeReviewCommand, Guid>
{
    public async Task<Guid> Handle(AnalyzeReviewCommand c, CancellationToken ct)
    {
        var existingAnalysis = await repo.FindByReviewIdAndStoreAsync(c.ReviewId, c.Store, ct);
        if (existingAnalysis is not null)
        {
            bool rawTextUnchanged = existingAnalysis.RawText == c.Text;
            bool dignityUnchanged = existingAnalysis.Dignity == c.Dignity;
            bool shortcomingsUnchanged = existingAnalysis.Shortcomings == c.Shortcomings;

            if (rawTextUnchanged && dignityUnchanged && shortcomingsUnchanged)
            {
                log.LogInformation("Existing analysis found for ReviewId={ReviewId}, Store={Store}; reusing it.",
                    c.ReviewId, c.Store);

                var existingDto = mapper.Map<ReviewAnalyzedData>((existingAnalysis, c.RequestId));
                await publisher.PublishReviewAnalyzedAsync(existingDto, ct);

                return existingAnalysis.Id;
            }
        }

        var promptBuilder = new StringBuilder();

        promptBuilder.AppendLine($"Відгук про {c.ProductTitle}:");
        promptBuilder.AppendLine(c.Text);

        if (!string.IsNullOrWhiteSpace(c.Dignity))
        {
            promptBuilder.AppendLine($"Переваги: {c.Dignity}");
        }

        if (!string.IsNullOrWhiteSpace(c.Shortcomings))
        {
            promptBuilder.AppendLine($"Недоліки: {c.Shortcomings}");
        }

        ReviewAnalysisResult result;
        try
        {
            result = await openAi.AnalyzeAsync(promptBuilder.ToString(), ct);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "OpenAI analysis failed for ReviewId={ReviewId}, Store={Store}", c.ReviewId, c.Store);
            throw;
        }

        var doc = new ReviewAnalysis
        {
            RequestId = c.RequestId,
            ReviewId = c.ReviewId,
            ProductId = c.ProductId,
            ProductTitle = c.ProductTitle,
            Store = c.Store,
            UserTitle = c.UserTitle,
            Mark = c.Mark,
            Dignity = c.Dignity,
            Shortcomings = c.Shortcomings,
            FromBuyer = c.FromBuyer,
            ReviewCreatedAt = c.CreatedAt,
            RawText = c.Text,

            ProductSentiment = result.Product.Sentiment,
            ProductSentimentScore = result.Product.SentimentScore,
            ProductSummary = result.Product.Summary,
            ProductEmotions = result.Product.Emotions,
            ProductKeywords = result.Product.Keywords,
            ProductPros = result.Product.Pros,
            ProductCons = result.Product.Cons,
            ProductUsageInsights = result.Product.UsageInsights,
            ProductAspectSentiments = result.Product.AspectSentiments,

            StoreSentiment = result.Store.Sentiment,
            StoreSentimentScore = result.Store.SentimentScore,
            StorePros = result.Store.Pros,
            StoreCons = result.Store.Cons
        };

        await repo.InsertAsync(doc, ct);

        var productAspectSentDto = result.Product.AspectSentiments
            .Select(x => new AspectSentimentItemData(
                x.Aspect,
                x.Sentiment switch
                {
                    Sentiment.Positive => "positive",
                    Sentiment.Negative => "negative",
                    _ => "neutral"
                },
                x.SentimentScore))
            .ToList();

        var dto = mapper.Map<ReviewAnalyzedData>(doc);

        await publisher.PublishReviewAnalyzedAsync(dto, ct);

        return doc.Id;
    }
}