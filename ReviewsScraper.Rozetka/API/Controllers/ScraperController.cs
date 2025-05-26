using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Controllers;

[ApiController]
[Route("api/scraper/rozetka")]
public sealed class ScraperController(IMediator mediator) : ControllerBase
{
    [HttpPost("reviews")]
    public async Task<IActionResult> FetchAll([FromBody] UrlDto dto, CancellationToken ct)
    {
        var count = await mediator.Send(new FetchReviewsCommand(dto.Url), ct);
        return Ok(new { Added = count });
    }

    public record UrlDto(string Url);
}