using System.Text.RegularExpressions;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Services;

public static partial class ProductLinkParser
{
    public static bool TryDetectStore(string url, out string store)
    {
        store = string.Empty;
        if (Rozetka().IsMatch(url)) store = "rozetka";
        else if (Foxtrot().IsMatch(url)) store = "foxtrot";
        else if (Allo().IsMatch(url)) store = "allo";
        return store != string.Empty;
    }

    [GeneratedRegex(@"https?://([a-z0-9-]+\.)*rozetka\.com\.ua", RegexOptions.IgnoreCase)]
    private static partial Regex Rozetka();
    [GeneratedRegex(@"https?://(?:www\.)?foxtrot\.com\.ua", RegexOptions.IgnoreCase)]
    private static partial Regex Foxtrot();
    [GeneratedRegex(@"https?://(?:www\.)?allo\.ua", RegexOptions.IgnoreCase)]
    private static partial Regex Allo();
}