using FluentValidation;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandValidator : AbstractValidator<FetchReviewsCommand>
{
    public FetchReviewsCommandValidator()
    {
        RuleFor(x => x.ProductUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var u) && u.Scheme.StartsWith("http"))
            .WithMessage("Невалідний URL");
    }
}