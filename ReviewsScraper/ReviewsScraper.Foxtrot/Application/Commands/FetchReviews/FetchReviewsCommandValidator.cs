using FluentValidation;

namespace ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Application.Commands.FetchReviews;

public sealed class FetchReviewsCommandValidator : AbstractValidator<FetchReviewsCommand>
{
    public FetchReviewsCommandValidator()
    {
        RuleFor(x => x.ProductUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var u)
                         && (u.Scheme == "http" || u.Scheme == "https"))
            .WithMessage("Невалідний URL");
    }
}