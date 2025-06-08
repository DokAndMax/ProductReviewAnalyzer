using MediatR;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Logging;

public sealed record CreateLogEntryCommand(LogEntry Entry) : IRequest;
