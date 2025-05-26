using MediatR;
using FluentValidation;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Models;
using ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Handlers;

public class AnalyzeProductCommandHandler(
    IApiGatewayClient api,
    IValidator<AnalyzeRequestModel> validator)
    : IRequestHandler<AnalyzeProductCommand, Unit>
{
    public async Task<Unit> Handle(AnalyzeProductCommand cmd, CancellationToken ct)
    {
        var model = cmd.Request;
        var validation = await validator.ValidateAsync(model, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        await api.ScrapeAsync(model);
        return Unit.Value;
    }
}