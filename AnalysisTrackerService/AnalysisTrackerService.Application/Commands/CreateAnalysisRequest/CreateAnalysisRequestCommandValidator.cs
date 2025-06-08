using FluentValidation;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CreateAnalysisRequest;

public class CreateAnalysisRequestCommandValidator : AbstractValidator<CreateAnalysisRequestCommand>
{
    public CreateAnalysisRequestCommandValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();

        RuleFor(r => r.ProductUrls)
            .NotEmpty().WithMessage("Має бути принаймні один URL")
            .Must(l => l.Count <= 20).WithMessage("Максимум 20 URL у batch‑запиті");

        RuleForEach(r => r.ProductUrls)
            .Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute))
            .WithMessage("Неправильний формат URL");
    }
}