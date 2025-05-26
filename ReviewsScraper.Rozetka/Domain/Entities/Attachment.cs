using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;

public class Attachment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReviewId { get; set; }

    [Required]
    public long ExternalId { get; set; }

    public string Url { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
}