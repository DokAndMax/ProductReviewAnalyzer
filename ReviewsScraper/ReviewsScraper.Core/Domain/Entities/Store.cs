namespace ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public ICollection<Product> Products { get; set; } = [];
    }
}
