using MediatR;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Commands.FetchReviews;

public sealed record FetchReviewsCommand(Guid RequestId, string ProductUrl) : IRequest<int>;