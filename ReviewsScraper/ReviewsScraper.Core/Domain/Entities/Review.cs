namespace ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

public class Review
{
    public int Id { get; set; }

    public long ExternalId { get; set; }

    public ICollection<Product> Products { get; set; } = [];

    public string UserTitle { get; set; } = default!;
    public int? Mark { get; set; }
    public string Text { get; set; } = default!;
    public string? Dignity { get; set; }
    public string? Shortcomings { get; set; }
    public bool FromBuyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = [];
}