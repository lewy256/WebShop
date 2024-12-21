using Microsoft.EntityFrameworkCore;
using ProductApi.Entities;
using ProductApi.Infrastructure.Configurations.Database;

namespace ProductApi.Infrastructure;

public partial class ProductContext : DbContext {
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<PriceHistory> PriceHistory { get; set; }
    public DbSet<WishlistItem> WishlistItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new PriceHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}