using MediatR;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Application.Commands.FetchReviews;

public sealed record FetchReviewsCommand(Guid RequestId, string ProductUrl) : IRequest<int>;