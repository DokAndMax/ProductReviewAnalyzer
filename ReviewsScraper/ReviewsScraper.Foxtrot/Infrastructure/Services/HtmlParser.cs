using HtmlAgilityPack;
using System.Globalization;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;

public sealed class HtmlParser
{
    public IEnumerable<Review> Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var items = doc.DocumentNode.SelectNodes("//div[@class='product-comment__item' and @data-comment-id]");
        if (items is null) yield break;

        foreach (var node in items)
        {
            var idStr = node.GetAttributeValue("data-comment-id", "0");
            if (!long.TryParse(idStr, out var extId)) continue;

            var userTitle = node.SelectSingleNode(".//div[@class='product-comment__item-title']")
                              ?.InnerText.Trim() ?? "Anonymous";

            int? rating = null;
            var ratingNum = node.SelectSingleNode(".//span[@class='product-comment__item-rating-num']")?.InnerText;
            if (ratingNum is not null && int.TryParse(ratingNum.Split('/')[0], out var r))
                rating = r;

            var dateText = node.SelectSingleNode(".//div[@class='product-comment__item-date']")?.InnerText.Trim();
            DateTime.TryParseExact(dateText, "dd.MM.yyyy", CultureInfo.GetCultureInfo("uk-UA"),
                DateTimeStyles.AssumeUniversal, out var createdAt);

            var text = HtmlEntity.DeEntitize(
                node.SelectSingleNode(".//div[@class='product-comment__item-text']")?.InnerText.Trim() ?? string.Empty);

            string? pros = null, cons = null;
            var infoLis = node.SelectNodes(".//ul[@class='product-comment__item-info']/li");
            if (infoLis is not null)
            {
                foreach (var li in infoLis)
                {
                    var label = li.SelectSingleNode("./label")?.InnerText.Trim().ToLowerInvariant();
                    var pText = HtmlEntity.DeEntitize(li.SelectSingleNode("./p")?.InnerText.Trim() ?? string.Empty);

                    if (label?.Contains("переваг") == true) pros = pText;
                    else if (label?.Contains("недолік") == true) cons = pText;
                }
            }

            yield return new Review
            {
                ExternalId = extId,
                UserTitle = userTitle,
                Mark = rating,
                Text = text,
                Dignity = pros,
                Shortcomings = cons,
                FromBuyer = true,                   // Foxtrot не дає прапор – умовно true
                CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt,
            };
        }
    }
}
