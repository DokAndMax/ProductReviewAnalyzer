using Refit;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Services;

public class ApiGatewayClient(IApiGatewayClient client) : IApiGatewayClient
{
    public Task<ApiResponse<CreatedResponse>> CreateRequestAsync(CreateRequestDto dto)
        => client.CreateRequestAsync(dto);

    public Task<AnalysisRequestDto> GetByIdAsync(Guid id)
        => client.GetByIdAsync(id);

    public Task<List<AnalysisRequestDto>> GetRequestsAsync(Guid? userId = null)
        => client.GetRequestsAsync(userId);
}