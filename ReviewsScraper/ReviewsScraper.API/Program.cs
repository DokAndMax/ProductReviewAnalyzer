using ProductReviewAnalyzer.Common.Persistence;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Application;
using ProductReviewAnalyzer.ReviewsScraper.Allo.Infrastructure;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure;
using ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Messaging;
using ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Application;
using ProductReviewAnalyzer.ReviewsScraper.Foxtrot.Infrastructure;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure;
using ReviewsScraper.API.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
})
    .AddApplicationPart(typeof(ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Controllers.ScraperController).Assembly)
    .AddApplicationPart(typeof(ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Controllers.ScraperController).Assembly)
    .AddApplicationPart(typeof(ProductReviewAnalyzer.ReviewsScraper.Rozetka.API.Controllers.ScraperController).Assembly)
    .AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddReviewsScraperCore()
    .AddReviewsScraperInfrastructure(builder.Configuration)
    .AddFoxtrotApplication()
    .AddFoxtrotInfrastructure(builder.Configuration)
    .AddAlloApplication()
    .AddAlloInfrastructure(builder.Configuration)
    .AddRozetkaApplication()
    .AddRozetkaInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration);

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