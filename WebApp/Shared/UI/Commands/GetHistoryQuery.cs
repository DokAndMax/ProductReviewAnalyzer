using MediatR;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Shared.UI.Commands;

public record GetHistoryQuery(Guid? UserId) : IRequest<List<AnalysisRequestDto>>;