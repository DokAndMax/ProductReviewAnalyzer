using MediatR;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Models;
using ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Handlers;

public class GetHistoryQueryHandler(IApiGatewayClient api) : IRequestHandler<GetHistoryQuery, List<HistoryRecordDto>>
{
    public async Task<List<HistoryRecordDto>> Handle(GetHistoryQuery _, CancellationToken ct)
        => await api.GetHistoryAsync();
}