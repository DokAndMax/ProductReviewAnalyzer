using MediatR;
using Microsoft.AspNetCore.SignalR;
using ProductReviewAnalyzer.AnalysisTrackerService.API.Hubs;
using ProductReviewAnalyzer.AnalysisTrackerService.API.Services;
using ProductReviewAnalyzer.AnalysisTrackerService.Application;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CompleteAnalysisRequest;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Commands.CreateAnalysisRequest;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Interfaces;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequestById;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.Queries.GetAnalysisRequests;
using ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure;
using ProductReviewAnalyzer.AnalysisTrackerService.Messaging;
using ProductReviewAnalyzer.Common.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IUrlDispatchService, HttpUrlDispatchService>();

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();
builder.Services.AddScoped<IAnalysisNotifier, SignalRAnalysisNotifier>();

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
        await migrator.MigrateAsync();
}

app.MapGet("/api/requests", async (IMediator mediator, Guid? userId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetAnalysisRequestsQuery(userId), ct)));

app.MapGet("/api/requests/{id:guid}",
    async (Guid id, IMediator mediator, CancellationToken ct) =>
    {
        var dto = await mediator.Send(new GetAnalysisRequestByIdQuery(id), ct);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    });

app.MapPost("/api/requests", async (CreateAnalysisRequestDto dto, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateAnalysisRequestCommand(dto.UserId, dto.ProductUrls), ct);
    return Results.Accepted($"/api/requests/{id}", new { id });
});

app.MapPost("/internal/requests/{id:guid}/complete", async (Guid id, CompleteAnalysisRequestDto dto, IMediator mediator, CancellationToken ct) =>
{
    await mediator.Send(new CompleteAnalysisRequestCommand(id, dto.DashboardUrl, dto.IsSuccess), ct);
    return Results.Ok();
});

app.MapHub<AnalysisHub>("/ws/analysis");

app.Run();

record CreateAnalysisRequestDto(Guid UserId, IReadOnlyList<string> ProductUrls);
record CompleteAnalysisRequestDto(string DashboardUrl, bool IsSuccess = true);