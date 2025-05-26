using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Infrastructure.Persistence;

public class RozetkaDbContext(DbContextOptions<RozetkaDbContext> options) : DbContext(options)
{
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Review>(cfg =>
        {
            cfg.HasKey(x => x.Id);

            cfg.HasIndex(x => x.ExternalId).IsUnique();

            cfg.Property(x => x.Text).HasColumnType("nvarchar(max)");
            cfg.Property(x => x.Dignity).HasColumnType("nvarchar(max)");
            cfg.Property(x => x.Shortcomings).HasColumnType("nvarchar(max)");
            cfg.HasMany(x => x.Attachments)
                .WithOne()
                .HasForeignKey(a => a.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Attachment>(cfg =>
        {
            cfg.HasKey(x => x.Id);
            cfg.HasIndex(x => x.ExternalId).IsUnique();
        });
    }
}