using System.Text.Json;
using ProductReviewAnalyzer.Contracts;

namespace ProductReviewAnalyzer.TableauExporterService.Features.PubSub;

internal static class DatasetBuilder
{
    public static string Build(AnalysisCompleted evt)
    {
        var payload = new
        {
            evt.AnalysisId,
            evt.GoodsId,
            evt.Store,
            evt.ProductSentiment,
            evt.StoreSentiment,
            evt.ProductPros,
            evt.ProductCons,
            evt.ProductCategories,
            evt.StorePros,
            evt.StoreCons,
            evt.AnalyzedAtUtc
        };

        return JsonSerializer.Serialize(payload);
    }
}