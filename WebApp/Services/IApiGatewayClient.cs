using Refit;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Services;

public interface IApiGatewayClient
{
    [Post("/api/requests")]
    Task<ApiResponse<CreatedResponse>> CreateRequestAsync([Body] CreateRequestDto dto);

    [Get("/api/requests")]
    Task<List<AnalysisRequestDto>> GetRequestsAsync([AliasAs("userId")] Guid? userId = null);
    
    [Get("/api/requests/{id}")]
    Task<AnalysisRequestDto> GetByIdAsync(Guid id);
}