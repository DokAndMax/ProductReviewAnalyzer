namespace ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public long ExternalId { get; set; }

        public string Title { get; set; } = default!;

        public int StoreId { get; set; }

        public Store Store { get; set; } = default!;

        public ICollection<Review> Reviews { get; set; } = [];
    }
}
