using MediatR;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

public record AnalyzeProductCommand(AnalyzeRequestModel Request) : IRequest<Unit>;