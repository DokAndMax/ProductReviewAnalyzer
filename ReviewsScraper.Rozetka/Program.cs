using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Filters;
using MassTransit;
using ProductReviewAnalyzer.Common.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((ctx, busCfg) =>
    {
        var mq = builder.Configuration.GetSection("RabbitMq");
        busCfg.Host(mq["Host"], mq["VirtualHost"], h =>
        {
            h.Username(mq["Username"]);
            h.Password(mq["Password"]);
        });

        busCfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var migrators = scope.ServiceProvider.GetServices<IDatabaseMigrator>();
    foreach (var migrator in migrators)
    {
        await migrator.MigrateAsync();
    }
}

app.MapControllers();
app.Run();