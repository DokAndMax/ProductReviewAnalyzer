using MediatR;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

public sealed record FetchReviewsCommand(string ProductUrl) : IRequest<int>;