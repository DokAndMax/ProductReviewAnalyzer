using MediatR;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Models;
using ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Handlers;

public class GetHistoryQueryHandler(IApiGatewayClient api) : IRequestHandler<GetHistoryQuery, List<AnalysisRequestDto>>
{
    private readonly IApiGatewayClient api = api;

    public async Task<List<AnalysisRequestDto>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
    {
        return await api.GetRequestsAsync(request.UserId);
    }
}
