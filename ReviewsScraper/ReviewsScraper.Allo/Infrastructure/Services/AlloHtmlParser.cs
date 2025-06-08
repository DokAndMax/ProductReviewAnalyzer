using HtmlAgilityPack;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;
using System.Text.RegularExpressions;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;

public sealed class AlloHtmlParser : IAlloHtmlParser
{
    private static readonly Regex contentIdsRegex = new("content_ids:\\s*'(?<id>\\d+)'", RegexOptions.Compiled);

    public bool TryExtractMeta(string html, out long productId, out string title)
    {
        productId = 0;
        title = string.Empty;

        var m = contentIdsRegex.Match(html);
        if (!m.Success || !long.TryParse(m.Groups["id"].Value, out productId))
        {
            return false;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var h1 = doc.DocumentNode.SelectSingleNode("//h1[@itemprop='name' and contains(@class,'p-view__header-title')]");
        if (h1 != null)
            title = HtmlEntity.DeEntitize(h1.InnerText.Trim());

        return productId > 0;
    }
}