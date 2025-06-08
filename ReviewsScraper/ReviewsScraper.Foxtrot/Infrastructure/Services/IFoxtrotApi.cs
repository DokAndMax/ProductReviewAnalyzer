using Refit;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure.Services;

public interface IFoxtrotApi
{
    [Get("/uk/product/getcommentsandquestions?catalogObjectId={oid}&classId={cid}&page={page}&itemsPerPage={ipp}&isQuestionOnly=false&brandTitle={brand}")]
    Task<string> GetReviewsAsync(
        [AliasAs("oid")] long catalogObjectId,
        [AliasAs("cid")] int classId,
        [AliasAs("page")] int page,
        [AliasAs("ipp")] int itemsPerPage,
        [AliasAs("brand")] string brandTitle,
        CancellationToken ct = default);
}