using Microsoft.EntityFrameworkCore;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Persistence;

public class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Store> Stores => Set<Store>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Store>(cfg =>
        {
            cfg.HasKey(s => s.Id);
            cfg.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(64);

            cfg.HasIndex(s => s.Name)
                    .IsUnique();
        });

        b.Entity<Product>(cfg =>
        {
            cfg.HasKey(p => p.Id)
                .IsClustered();
            cfg.HasIndex(p => new { p.ExternalId, p.StoreId });
            cfg.Property(p => p.ExternalId)
                .IsRequired();
            cfg.Property(p => p.Title)
                .HasMaxLength(512);
            cfg.HasOne(p => p.Store)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Review>(cfg =>
        {
            cfg.HasKey(r => r.Id);
            cfg.HasIndex(r => r.ExternalId)
                        .IsUnique();
            cfg.Property(r => r.Text)
                .HasColumnType("nvarchar(max)");
            cfg.Property(r => r.Dignity)
                .HasColumnType("nvarchar(max)");
            cfg.Property(r => r.Shortcomings)
                .HasColumnType("nvarchar(max)");

            cfg.HasMany(r => r.Products)
                .WithMany(p => p.Reviews)
                .UsingEntity<Dictionary<string, object>>(
                    "ReviewProduct",
                    j => j
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Review>()
                        .WithMany()
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("ReviewId", "ProductId");
                        j.ToTable("ReviewProduct");
                    });
        });

        b.Entity<Attachment>(cfg =>
        {
            cfg.HasKey(a => a.Id);
            cfg.HasIndex(a => a.ExternalId).IsUnique();

            cfg.Property(a => a.Url)
                .IsRequired();

            cfg.HasOne<Review>()
                .WithMany(r => r.Attachments)
                .HasForeignKey(a => a.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}