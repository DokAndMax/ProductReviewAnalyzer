using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.Queries.GetReviews;

public sealed class GetReviewsQueryHandler(
    ReviewsDbContext db,
    IMapper mapper)
    : IRequestHandler<GetReviewsQuery, IReadOnlyList<ReviewDto>>
{
    public async Task<IReadOnlyList<ReviewDto>> Handle(
        GetReviewsQuery request,
        CancellationToken ct)
    {
        var reviews = await db.Reviews
            .Where(r => r.Products.Any(p => p.ExternalId == request.ProductId))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return reviews.Select(mapper.Map<ReviewDto>).ToList();
    }
}