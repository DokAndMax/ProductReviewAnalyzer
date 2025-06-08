using MediatR;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.Queries.GetReviews;

public sealed record GetReviewsQuery(long ProductId) : IRequest<IReadOnlyList<ReviewDto>>;