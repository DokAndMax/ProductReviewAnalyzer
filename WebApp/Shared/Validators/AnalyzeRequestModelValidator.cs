using FluentValidation;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Shared.Validators;

public class AnalyzeRequestModelValidator : AbstractValidator<AnalyzeRequestModel>
{
    public AnalyzeRequestModelValidator()
    {
        RuleFor(x => x.Urls)
            .NotEmpty().WithMessage("URL обов’язкові")
            .Must(text =>
            {
                var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                return lines.All(u => Uri.IsWellFormedUriString(u.Trim(), UriKind.Absolute));
            }).WithMessage("Список містить некоректні URL");
    }
}