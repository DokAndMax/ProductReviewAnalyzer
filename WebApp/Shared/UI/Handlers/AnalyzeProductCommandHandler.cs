using MediatR;
using FluentValidation;
using ProductReviewAnalyzer.WebApp.Services;
using ProductReviewAnalyzer.WebApp.Shared.Models;
using ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Handlers;

public class AnalyzeProductCommandHandler(
    IApiGatewayClient api,
    IValidator<AnalyzeRequestModel> validator)
    : IRequestHandler<AnalyzeProductCommand, Guid>
{
    public async Task<Guid> Handle(AnalyzeProductCommand cmd, CancellationToken ct)
    {
        var model = cmd.Request;
        var validation = await validator.ValidateAsync(model, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var urls = model.Urls.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(u => u.Trim())
            .ToList();

        var dto = new CreateRequestDto(cmd.UserId, urls);
        var resp = await api.CreateRequestAsync(dto);
        return resp.Content.Id;
    }
}