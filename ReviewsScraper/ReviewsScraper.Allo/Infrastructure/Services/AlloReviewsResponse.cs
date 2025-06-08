namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;

public sealed class AlloReviewsResponse
{
    public int count_items { get; init; }
    public required Item[] items { get; init; }
    public int current_page { get; init; }

    public sealed class Item
    {
        public long item_id { get; init; }
        public string type { get; init; } = string.Empty;
        public string author { get; init; } = string.Empty;
        public DateLabel date_label { get; init; } = new();
        public string text { get; init; } = string.Empty;
        public Rating rating { get; init; } = new();
        public bool? was_bought_in_allo { get; init; }
        public string advantages { get; init; } = string.Empty;
        public string flaws { get; init; } = string.Empty;

        public sealed class DateLabel
        {
            public string date_published { get; init; } = string.Empty;
        }

        public sealed class Rating
        {
            public int? value { get; init; }
        }
    }
}