using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.HistoryService.Domain.Entities;

namespace ProductReviewAnalyzer.HistoryService.Infrastructure.Persistence;

public class HistoryDbContext(DbContextOptions<HistoryDbContext> options) : DbContext(options)
{
    public DbSet<HistoryRecord> HistoryRecords => Set<HistoryRecord>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<HistoryRecord>(cfg =>
        {
            cfg.ToTable("HistoryRecords");
            cfg.HasKey(h => h.Id);
            cfg.Property(h => h.Url).IsRequired().HasMaxLength(2048);
            cfg.Property(h => h.Status).IsRequired();
            cfg.Property(h => h.RequestedAtUtc).IsRequired();
        });
    }
}