using System.Text.Json;
using Tableau.HyperAPI;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;

public sealed class HyperFileWriter : IHyperFileWriter
{
    public byte[] Write(IEnumerable<TableDefinition> tableDefinitions, string jsonDataset)
    {
        var tmpPath = Path.GetTempFileName() + ".hyper";

        using var hyper = new HyperProcess(Telemetry.DoNotSendUsageDataToTableau);
        using var connection = new Connection(hyper.Endpoint, tmpPath, CreateMode.CreateAndReplace);

        connection.Catalog.CreateSchema("Extract");
        foreach (var def in tableDefinitions)
        {
            connection.Catalog.CreateTable(def);
        }

        var mainTableName = new TableName("Extract", "analysis");
        connection.ExecuteCommand($@"
                ALTER TABLE {mainTableName}
                ADD ASSUMED UNIQUE (request_id);
            ");

        var dimNames = new[]
        {
            "product_emotions",
            "product_keywords",
            "product_pros",
            "product_cons",
            "product_usage",
            "product_aspect_sentiments",
            "store_pros",
            "store_cons"
        };

        foreach (var table in dimNames)
        {
            var dimTableName = new TableName("Extract", table);
            connection.ExecuteCommand($@"
                    ALTER TABLE {dimTableName}
                    ADD ASSUMED PRIMARY KEY (dim_id);
                ");
            connection.ExecuteCommand($@"
                    ALTER TABLE {mainTableName}
                    ADD ASSUMED FOREIGN KEY ({table}_dim_id) REFERENCES {dimTableName}(dim_id);
                ");
        }

        using var document = JsonDocument.Parse(jsonDataset);
        var root = document.RootElement;
        var reviewsElement = root.GetProperty("Reviews");

        short dimId = 1;

        foreach (var review in reviewsElement.EnumerateArray())
        {
            InsertIntoDimCategoryTable(
                review, "ProductEmotions", "product_emotions", connection, dimId);

            InsertIntoDimCategoryTable(
                review, "ProductKeywords", "product_keywords", connection, dimId);

            InsertIntoDimTable(
                review, "ProductPros", "product_pros", connection, dimId);

            InsertIntoDimTable(
                review, "ProductCons", "product_cons", connection, dimId);

            InsertIntoDimTable(
                review, "ProductUsageInsights", "product_usage", connection, dimId);

            InsertIntoAspectSentiments(
                review, "ProductAspectSentiments", "product_aspect_sentiments", connection, dimId);

            InsertIntoDimTable(
                review, "StorePros", "store_pros", connection, dimId);

            InsertIntoDimTable(
                review, "StoreCons", "store_cons", connection, dimId);

            dimId++;
        }

        dimId = 1;
        using var inserter = new Inserter(connection, connection.Catalog.GetTableDefinition(mainTableName));
        foreach (var review in reviewsElement.EnumerateArray())
        {
            var requestId = review.GetProperty("RequestId").GetGuid().ToString();
            var reviewId = review.GetProperty("ReviewId").GetInt32();
            var productId = review.GetProperty("ProductId").GetInt64();
            var store = review.GetProperty("Store").GetString()!;
            var createdAtUtc = review.GetProperty("CreatedAtUtc").GetDateTime();
            var productSent = review.GetProperty("ProductSentimentScore").GetDouble();
            var storeSent = review.GetProperty("StoreSentimentScore").GetDouble();
            var productSummary = review.GetProperty("ProductSummary").GetString()!;
            var analyzedAtUtc = review.GetProperty("AnalyzedAtUtc").GetDateTime();

            inserter.AddRow(
                requestId,
                reviewId,
                productId,
                store,
                (Timestamp)createdAtUtc,
                productSent,
                storeSent,
                productSummary,
                dimId, // product_emotions_dim_id
                dimId, // product_keywords_dim_id
                dimId, // product_pros_dim_id
                dimId, // product_cons_dim_id
                dimId, // product_usage_dim_id
                dimId, // product_aspect_sentiments_dim_id
                dimId, // store_pros_dim_id
                dimId, // store_cons_dim_id
                (Timestamp)analyzedAtUtc
            );

            dimId++;
        }

        inserter.Execute();

        connection.Close();
        hyper.Close();

        var bytes = File.ReadAllBytes(tmpPath);
        File.Delete(tmpPath);
        return bytes;
    }


    private static void InsertIntoDimTable(
        JsonElement parent,
        string jsonPropName,
        string tableKey,
        Connection conn,
        short dimId)
    {
        if (!parent.TryGetProperty(jsonPropName, out var arr) || arr.ValueKind != JsonValueKind.Array)
            return;

        var def = conn.Catalog.GetTableDefinition(new TableName("Extract", tableKey));
        using var ins = new Inserter(conn, def);
        foreach (var element in arr.EnumerateArray())
        {
            var textVal = element.GetProperty("Text").GetString()!;
            var categoryVal = element.GetProperty("Category").GetString()!;

            if (tableKey == "product_usage")
            {
                ins.AddRow(dimId, textVal, categoryVal);
            }
            else
            {
                var scoreVal = element.GetProperty("SentimentScore").GetDouble();
                ins.AddRow(dimId, textVal, categoryVal, scoreVal);
            }

        }
        ins.Execute();
    }

    private static void InsertIntoDimCategoryTable(
        JsonElement parent,
        string jsonPropName,
        string tableKey,
        Connection conn,
        short dimId)
    {
        if (!parent.TryGetProperty(jsonPropName, out var arr) || arr.ValueKind != JsonValueKind.Array)
            return;

        var def = conn.Catalog.GetTableDefinition(new TableName("Extract", tableKey));
        using var ins = new Inserter(conn, def);
        foreach (var element in arr.EnumerateArray())
        {
            var categoryVal = element.GetString()!;
            ins.AddRow(dimId, categoryVal);
        }
        ins.Execute();
    }

    private static void InsertIntoAspectSentiments(
        JsonElement parent,
        string jsonPropName,
        string tableKey,
        Connection conn,
        short dimId)
    {
        if (!parent.TryGetProperty(jsonPropName, out var arr) || arr.ValueKind != JsonValueKind.Array)
            return;

        var def = conn.Catalog.GetTableDefinition(new TableName("Extract", tableKey));
        using var ins = new Inserter(conn, def);
        foreach (var element in arr.EnumerateArray())
        {
            var aspectVal = element.GetProperty("Aspect").GetString()!;
            var sentimentVal = element.GetProperty("Sentiment").GetString()!;
            var scoreVal = element.GetProperty("SentimentScore").GetDouble();

            ins.AddRow(dimId, aspectVal, sentimentVal, scoreVal);
        }
        ins.Execute();
    }
}
