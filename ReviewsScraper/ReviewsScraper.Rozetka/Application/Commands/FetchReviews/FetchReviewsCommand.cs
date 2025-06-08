using MediatR;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

public sealed record FetchReviewsCommand(Guid RequestId, string ProductUrl) : IRequest<int>;