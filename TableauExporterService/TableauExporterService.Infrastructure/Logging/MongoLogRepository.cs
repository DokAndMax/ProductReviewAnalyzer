using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ProductReviewAnalyzer.TableauExporterService.Features.Logging;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Logging;

internal sealed class MongoLogRepository : ILogRepository
{
    private readonly IMongoCollection<LogEntry> collection;
    public MongoLogRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Mongo")
            ?? throw new InvalidOperationException("Mongo connection string not configured");

        if (!BsonClassMap.IsClassMapRegistered(typeof(LogEntry)))
        {
            BsonClassMap.RegisterClassMap<LogEntry>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(x => x.AnalysisId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            });
        }

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("tableau_exporter");
        collection = database.GetCollection<LogEntry>("logs");
    }

    public Task InsertAsync(LogEntry entry, CancellationToken ct) =>
        collection.InsertOneAsync(entry, cancellationToken: ct);
}