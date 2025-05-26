using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;

internal sealed class MongoLogRepository : ILogRepository
{
    private readonly IMongoCollection<LogEntry> _col;
    public MongoLogRepository(IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("Mongo")
                 ?? throw new InvalidOperationException("Mongo connection string not configured");
        var db = new MongoClient(cs).GetDatabase("tableau_exporter");
        _col = db.GetCollection<LogEntry>("logs");
    }

    public Task InsertAsync(LogEntry entry, CancellationToken ct) =>
        _col.InsertOneAsync(entry, cancellationToken: ct);
}