using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ProductReviewAnalyzer.AnalysisTrackerService.API.Services;

public class QueryStringUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var http = connection.GetHttpContext();
        if (http != null && http.Request.Query.TryGetValue("userId", out var userIdValues))
        {
            return userIdValues.First();
        }
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
