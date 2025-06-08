using HtmlAgilityPack;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;

public sealed class AlloReviewsParser : IAlloReviewsParser
{
    public IEnumerable<Review> Parse(AlloReviewsResponse resp)
    {
        foreach (var i in resp.items)
        {
            if (i.type != "review") continue;

            var createdAt = DateTime.TryParse(i.date_label.date_published, out var dt)
                ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                : DateTime.UtcNow;

            yield return new Review
            {
                ExternalId = i.item_id,
                UserTitle = HtmlEntity.DeEntitize(i.author),
                Mark = i.rating.value,
                Text = HtmlEntity.DeEntitize(i.text),
                Dignity = string.IsNullOrWhiteSpace(i.advantages) ? null : HtmlEntity.DeEntitize(i.advantages),
                Shortcomings = string.IsNullOrWhiteSpace(i.flaws) ? null : HtmlEntity.DeEntitize(i.flaws),
                FromBuyer = i.was_bought_in_allo ?? false,
                CreatedAt = createdAt
            };
        }
    }
}