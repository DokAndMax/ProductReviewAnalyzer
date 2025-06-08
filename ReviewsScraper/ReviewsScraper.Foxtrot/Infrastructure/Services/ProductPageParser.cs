using HtmlAgilityPack;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;

public sealed class ProductPageParser
{
    public record Meta(long CatalogObjectId, int ClassId, string BrandTitle, string Title);

    public Meta? Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@id='product-page-title']");
        var title = titleNode?.GetAttributeValue("data-full-title", null)
                    ?? titleNode?.InnerText.Trim()
                    ?? string.Empty;

        var btn = doc.DocumentNode.SelectSingleNode("//button[contains(@class,'product-buy-button')]");
        if (btn is null) return null;

        var idOk = long.TryParse(btn.GetAttributeValue("data-id", "0"), out var oid);
        var clsOk = int.TryParse(btn.GetAttributeValue("data-classid", "0"), out var cid);

        if (!idOk || !clsOk) return null;

        var brand = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

        return new Meta(oid, cid, brand, title);
    }
}