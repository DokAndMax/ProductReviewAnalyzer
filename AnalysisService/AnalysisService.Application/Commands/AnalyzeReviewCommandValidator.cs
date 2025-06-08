using FluentValidation;

namespace ProductReviewAnalyzer.AnalysisService.Application.Commands;

public class AnalyzeReviewCommandValidator : AbstractValidator<AnalyzeReviewCommand>
{
    public AnalyzeReviewCommandValidator()
    {
        RuleFor(c => c.ReviewId).GreaterThan(0);
        RuleFor(c => c.ProductId).GreaterThan(0);
        RuleFor(c => c.ProductTitle).NotEmpty();
        RuleFor(c => c.UserTitle).NotEmpty();
        RuleFor(c => c.Text).NotEmpty().MaximumLength(20_000);
    }
}