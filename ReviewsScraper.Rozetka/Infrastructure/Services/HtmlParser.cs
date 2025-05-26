using HtmlAgilityPack;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;
using System.Globalization;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public sealed class HtmlParser
{
    private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");
    private static readonly string[] DateFormats = { "d MMMM yyyy", "dd MMMM yyyy" };

    public IEnumerable<Review> Parse(RozetkaCommentsResponse resp)
    {
        foreach (var c in resp.data.comments)
        {
            var dateStr = c.created.pop_date;
            if (!DateTime.TryParseExact(dateStr, DateFormats, UkCulture, DateTimeStyles.None, out var createdAt))
            {
                createdAt = DateTime.Parse(dateStr, UkCulture);
            }

            yield return new Review
            {
                ExternalId = c.id,
                ProductId = long.Parse(c.goods_id),
                UserTitle = HtmlEntity.DeEntitize(c.usertitle),
                Mark = c.mark,
                Text = HtmlEntity.DeEntitize(c.text),
                Dignity = string.IsNullOrWhiteSpace(c.dignity) ? null
                    : HtmlEntity.DeEntitize(c.dignity),
                Shortcomings = string.IsNullOrWhiteSpace(c.shortcomings) ? null
                    : HtmlEntity.DeEntitize(c.shortcomings),
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