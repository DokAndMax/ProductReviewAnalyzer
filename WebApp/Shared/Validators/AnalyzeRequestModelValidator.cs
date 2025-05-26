using FluentValidation;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Shared.Validators;

public class AnalyzeRequestModelValidator : AbstractValidator<AnalyzeRequestModel>
{
    public AnalyzeRequestModelValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("URL не може бути порожнім")
            .Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute))
            .WithMessage("Невірний формат URL");
    }
}