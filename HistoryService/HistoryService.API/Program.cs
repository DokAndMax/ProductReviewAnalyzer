using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.Common.Persistence;
using ProductReviewAnalyzer.HistoryService.Application;
using ProductReviewAnalyzer.HistoryService.Application.Commands.CreateHistoryRecord;
using ProductReviewAnalyzer.HistoryService.Application.Queries.GetHistoryRecords;
using ProductReviewAnalyzer.HistoryService.Domain.ValueObjects;
using ProductReviewAnalyzer.HistoryService.Infrastructure;
using ProductReviewAnalyzer.HistoryService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var migrators = scope.ServiceProvider.GetServices<IDatabaseMigrator>();
    foreach (var migrator in migrators)
    {
        await migrator.MigrateAsync();
    }
}

app.MapGet("/api/history",
    async (IMediator mediator, CancellationToken ct)
        => Results.Ok(await mediator.Send(new GetHistoryRecordsQuery(), ct)));

app.MapPost("/api/history",
    async (CreateHistoryRecordRequest req, IMediator mediator, CancellationToken ct) =>
    {
        var id = await mediator.Send(new CreateHistoryRecordCommand(req.Url, req.Status), ct);
        return Results.Created($"/api/history/{id}", new { id });
    });

app.Run();

record CreateHistoryRecordRequest(string Url, RequestStatus Status);