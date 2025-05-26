namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.ValueObjects;

public class Attachment
{
    public long Id { get; set; }
    public long ReviewId { get; set; }
    public string Url { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
}