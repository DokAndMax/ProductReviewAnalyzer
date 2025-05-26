using Refit;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Services;

public class ApiGatewayClient(IApiGatewayClient client) : IApiGatewayClient
{
    public Task<ApiResponse<object>> ScrapeAsync(AnalyzeRequestModel request)
        => client.ScrapeAsync(request);

    public Task<List<HistoryRecordDto>> GetHistoryAsync()
        => client.GetHistoryAsync();
}