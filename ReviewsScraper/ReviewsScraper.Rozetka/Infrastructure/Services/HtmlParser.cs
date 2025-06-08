using HtmlAgilityPack;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public sealed class HtmlParser
{
    public IEnumerable<Review> Parse(RozetkaCommentsResponse resp)
    {
        foreach (var c in resp.data.comments)
        {
            var createdAt = new DateTime(
                int.Parse(c.created.year, System.Globalization.CultureInfo.InvariantCulture),
                c.created.month,
                int.Parse(c.created.day, System.Globalization.CultureInfo.InvariantCulture),
                0, 0, 0, DateTimeKind.Utc);

            yield return new Review
            {
                ExternalId = c.id,
                UserTitle = HtmlEntity.DeEntitize(c.usertitle),
                Mark = c.mark,
                Text = HtmlEntity.DeEntitize(c.text),
                Dignity = string.IsNullOrWhiteSpace(c.dignity) ? null : HtmlEntity.DeEntitize(c.dignity),
                Shortcomings = string.IsNullOrWhiteSpace(c.shortcomings) ? null : HtmlEntity.DeEntitize(c.shortcomings),
                FromBuyer = c.from_buyer,
                CreatedAt = createdAt,
                Attachments = c.attachments.Select(a => new Attachment
                {
                    ExternalId = a.id,
                    Url = a.preview.src,
                    Width = a.preview.width,
                    Height = a.preview.height
                }).ToList()
            };
        }
    }
}