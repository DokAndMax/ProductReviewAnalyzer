using Refit;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;

[Headers("x-requested-with: XMLHttpRequest", "x-use-nuxt: 1")]
public interface IAlloApi
{
    [Get("/ua/discussion/reviewQuestion/update/?tab_id=reviews&pagination={pagination}&showMoreComments=true&product_id={product_id}&isAjax=1&currentLocale=uk_UA")]
    Task<AlloReviewsResponse> GetReviewsAsync(
        [AliasAs("product_id")] long productId,
        [AliasAs("pagination")] int page = 1,
        CancellationToken ct = default);
}