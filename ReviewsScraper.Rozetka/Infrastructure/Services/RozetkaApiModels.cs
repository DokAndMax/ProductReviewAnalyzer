using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Services;

public sealed class RozetkaCommentsResponse
{
    public required Data data { get; init; }

    public sealed class Data
    {
        public required Comment[] comments { get; init; }
    }

    public sealed class Comment
    {
        public long id { get; init; }
        public string goods_id { get; init; } = string.Empty;
        public string usertitle { get; init; } = string.Empty;
        [JsonConverter(typeof(NullableIntConverter))]
        public int? mark { get; init; }
        public string text { get; init; } = string.Empty;
        public string dignity { get; init; } = string.Empty;
        public string shortcomings { get; init; } = string.Empty;
        public bool from_buyer { get; init; }
        public Created created { get; init; } = new();
        public AttachmentDto[] attachments { get; init; } = [];
    }

    public sealed class Created
    {
        public string pop_date { get; init; } = string.Empty;
    }

    public sealed class AttachmentDto
    {
        public long id { get; init; }
        public Preview preview { get; init; } = new();
        public sealed class Preview { public string src { get; init; } = string.Empty; public int width { get; init; } public int height { get; init; } }
    }

    private sealed class NullableIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
        {
            return r.TokenType switch
            {
                JsonTokenType.Number => r.GetInt32(),
                JsonTokenType.String when int.TryParse(r.GetString(), out var v) => v,
                JsonTokenType.Null => null,
                _ => null
            };
        }
        public override void Write(Utf8JsonWriter w, int? v, JsonSerializerOptions o) =>
            w.WriteNumberValue(v ?? 0);
    }
}