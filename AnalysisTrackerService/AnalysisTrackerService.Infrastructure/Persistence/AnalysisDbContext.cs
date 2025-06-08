using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure.Persistence;

public class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : DbContext(options)
{
    public DbSet<AnalysisRequest> AnalysisRequests => Set<AnalysisRequest>();
    public DbSet<AnalysisItem> AnalysisItems => Set<AnalysisItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AnalysisRequest>(cfg =>
        {
            cfg.ToTable("AnalysisRequests");
            cfg.HasKey(r => r.Id);
            cfg.Property(r => r.UserId).IsRequired();
            cfg.Property(r => r.Status).IsRequired();
            cfg.Property(r => r.CreatedAtUtc).IsRequired();
            cfg.Property(r => r.DashboardUrl).HasMaxLength(1024);
        });

        b.Entity<AnalysisItem>(cfg =>
        {
            cfg.ToTable("AnalysisItems");
            cfg.HasKey(i => i.Id);
            cfg.Property(i => i.ProductUrl).IsRequired().HasMaxLength(2048);
            cfg.Property(i => i.RequestId).IsRequired();
            cfg.HasIndex(i => i.RequestId);
            cfg.HasOne(i => i.Request)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}