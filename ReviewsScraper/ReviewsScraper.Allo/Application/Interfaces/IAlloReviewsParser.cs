using ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure.Services;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Allo.Application.Interfaces;

public interface IAlloReviewsParser
{
    IEnumerable<Review> Parse(AlloReviewsResponse resp);
}