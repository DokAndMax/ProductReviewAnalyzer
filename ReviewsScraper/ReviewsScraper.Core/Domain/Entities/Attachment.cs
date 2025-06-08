namespace ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public long ExternalId { get; set; }
    public string Url { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
}