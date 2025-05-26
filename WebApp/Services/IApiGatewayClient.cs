using Refit;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Services;

public interface IApiGatewayClient
{
    [Post("/api/scraper/rozetka/reviews")]
    Task<ApiResponse<object>> ScrapeAsync([Body] AnalyzeRequestModel request);

    [Get("/api/history")]
    Task<List<HistoryRecordDto>> GetHistoryAsync();
}