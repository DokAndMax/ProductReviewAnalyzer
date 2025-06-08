namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;

public interface IAlloHtmlParser
{
    bool TryExtractMeta(string html, out long productId, out string title);
}