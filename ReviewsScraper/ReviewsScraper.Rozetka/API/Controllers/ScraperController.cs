using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Controllers;

[ApiController]
[Route("api/scraper/rozetka")]
public sealed class ScraperController(IServiceScopeFactory scopeFactory,
    ILogger<ScraperController> logger) : ControllerBase
{
    [HttpPost("reviews")]
    public IActionResult FetchAll([FromBody] RequestDto dto)
    {
        logger.LogInformation(
            "Received fetch request: RequestId={RequestId}, Url={Url}",
            dto.RequestId,
            dto.Url);

        var scope = scopeFactory.CreateScope();
        var scopedProvider = scope.ServiceProvider;
        var mediator = scopedProvider.GetRequiredService<IMediator>();

        _ = Task.Run(async () =>
        {
            try
            {
                logger.LogDebug(
                    "Starting background task for Rozetka FetchReviews: RequestId={RequestId}",
                    dto.RequestId);

                var affectedCount = await mediator.Send(new FetchReviewsCommand(dto.RequestId, dto.Url));

                logger.LogInformation(
                    "Rozetka FetchReviews completed successfully: RequestId={RequestId}, ReviewsProcessed={Count}",
                    dto.RequestId,
                    affectedCount);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while fetching reviews in Rozetka Scraper: RequestId={RequestId}, Exception={Message}",
                    dto.RequestId,
                    ex.Message);
            }
            finally
            {
                logger.LogDebug(
                    "Disposing scope for Rozetka Scraper: RequestId={RequestId}",
                    dto.RequestId);

                scope.Dispose();
            }
        });

        return Accepted(new { Message = "Скрапінг успішно запущено" });
    }


    public record RequestDto(Guid RequestId, string Url);
}