using MediatR;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.DTOs;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Queries.GetReviews;

public sealed record GetReviewsQuery(long ProductId) : IRequest<IReadOnlyList<ReviewDto>>;