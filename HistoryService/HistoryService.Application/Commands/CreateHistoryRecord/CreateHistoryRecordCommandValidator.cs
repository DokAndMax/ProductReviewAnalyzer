using FluentValidation;

namespace ProductReviewAnalyzer.HistoryService.Application.Commands.CreateHistoryRecord;

public class CreateHistoryRecordCommandValidator : AbstractValidator<CreateHistoryRecordCommand>
{
    public CreateHistoryRecordCommandValidator()
    {
        RuleFor(r => r.Url).NotEmpty().MaximumLength(2048).Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute));
        RuleFor(r => r.Status).IsInEnum();
    }
}