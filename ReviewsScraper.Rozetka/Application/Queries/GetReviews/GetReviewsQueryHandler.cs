using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Persistence;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Queries.GetReviews;

public sealed class GetReviewsQueryHandler(RozetkaDbContext db, IMapper mapper)
    : IRequestHandler<GetReviewsQuery, IReadOnlyList<ReviewDto>>
{
    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await db.Reviews
            .Where(r => r.ProductId == request.ProductId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return reviews.Select(mapper.Map<ReviewDto>).ToList();
    }
}