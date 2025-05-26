using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public long ExternalId { get; set; }

    public long ProductId { get; set; }
    public string UserTitle { get; set; } = default!;
    public int? Mark { get; set; }
    public string Text { get; set; } = default!;
    public string? Dignity { get; set; }
    public string? Shortcomings { get; set; }
    public bool FromBuyer { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = [];
}