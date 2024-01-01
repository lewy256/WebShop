using Microsoft.EntityFrameworkCore;
using ProductApi.Model.Entities;

namespace ProductApi.Model;

public partial class ProductContext : DbContext {
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<PriceHistory> PriceHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        modelBuilder.ApplyConfiguration(new Configurations.ProductConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.PriceHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CategoryConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}