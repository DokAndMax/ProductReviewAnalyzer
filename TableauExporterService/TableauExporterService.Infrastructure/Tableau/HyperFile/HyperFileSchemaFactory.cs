using Tableau.HyperAPI;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;

public sealed class HyperFileSchemaFactory : IHyperFileSchemaFactory
{
    public IReadOnlyCollection<TableDefinition> Create()
    {
        var mainTable = new TableDefinition(new TableName("Extract", "analysis"))
            .AddColumn("request_id", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("review_id", SqlType.BigInt(), Nullability.NotNullable)
            .AddColumn("product_id", SqlType.BigInt(), Nullability.NotNullable)
            .AddColumn("store", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("created_at_utc", SqlType.Timestamp(), Nullability.NotNullable)
            .AddColumn("product_sentiment_score", SqlType.Double(), Nullability.NotNullable)
            .AddColumn("store_sentiment_score", SqlType.Double(), Nullability.NotNullable)
            .AddColumn("product_summary", SqlType.Text(), Nullability.NotNullable)

            .AddColumn("product_emotions_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("product_keywords_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("product_pros_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("product_cons_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("product_usage_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("product_aspect_sentiments_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("store_pros_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("store_cons_dim_id", SqlType.SmallInt(), Nullability.Nullable)
            .AddColumn("analyzed_at_utc", SqlType.Timestamp(), Nullability.NotNullable);

        var dimProductEmotions = new TableDefinition(new TableName("Extract", "product_emotions"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("emotion", SqlType.Text(), Nullability.NotNullable);

        var dimProductKeywords = new TableDefinition(new TableName("Extract", "product_keywords"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("keyword", SqlType.Text(), Nullability.NotNullable);

        var dimProductPros = new TableDefinition(new TableName("Extract", "product_pros"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("text", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("category", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment_score", SqlType.Double(), Nullability.NotNullable);

        var dimProductCons = new TableDefinition(new TableName("Extract", "product_cons"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("text", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("category", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment_score", SqlType.Double(), Nullability.NotNullable);

        var dimProductUsage = new TableDefinition(new TableName("Extract", "product_usage"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("text", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("category", SqlType.Text(), Nullability.NotNullable);

        var dimProductAspectSentiments = new TableDefinition(new TableName("Extract", "product_aspect_sentiments"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("aspect", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment_score", SqlType.Double(), Nullability.NotNullable);

        var dimStorePros = new TableDefinition(new TableName("Extract", "store_pros"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("text", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("category", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment_score", SqlType.Double(), Nullability.NotNullable);

        var dimStoreCons = new TableDefinition(new TableName("Extract", "store_cons"))
            .AddColumn("dim_id", SqlType.SmallInt(), Nullability.NotNullable)
            .AddColumn("text", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("category", SqlType.Text(), Nullability.NotNullable)
            .AddColumn("sentiment_score", SqlType.Double(), Nullability.NotNullable);

        return
        [
            mainTable,
            dimProductEmotions,
            dimProductKeywords,
            dimProductPros,
            dimProductCons,
            dimProductUsage,
            dimProductAspectSentiments,
            dimStorePros,
            dimStoreCons
        ];
    }
}