using Microsoft.EntityFrameworkCore;
using ProductApi.Model.Entities;

namespace ProductApi.Model;

public partial class ProductContext : DbContext {
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) {
    }

    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<ProductReview> ProductReview { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Category>()
            .ToContainer("Categories")
            .HasPartitionKey(p => p.Discriminator)
            .HasDiscriminator(d => d.Discriminator);

        modelBuilder.Entity<Category>()
            .Property(p => p.CategoryID)
            .HasConversion<string>();

        modelBuilder.Entity<Category>()
            .Property(x => x.Id)
            .HasConversion<string>()
            .ToJsonProperty("id");


        modelBuilder.Entity<Product>()
            .ToContainer("Products2")
            .HasPartitionKey(p => p.CategoryID)
            .HasNoDiscriminator()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Product>()
            .Property(p => p.CategoryID)
            .HasConversion<string>();

        modelBuilder.Entity<Product>()
            .Property(x => x.Id)
            .HasConversion<string>()
            .ToJsonProperty("id");


        /*            modelBuilder.Entity<ProductPriceHistory>()
                        .ToContainer("ProductPriceHistory")
                        .HasManualThroughput(1000)
                        .HasPartitionKey(p => p.ProductID)
                        .HasNoDiscriminator();*/

        /*        modelBuilder.Entity<Order>().OwnsMany(i => i.OrderItem);

                modelBuilder.Entity<Customer>().OwnsMany(i => i.CustomerAddresses);*/
        /*

              modelBuilder.Entity<Family>()
                  .OwnsMany(f => f.Children)
                      .OwnsMany(c => c.Pets);

              modelBuilder.Entity<Family>()
                  .OwnsOne(f => f.Address);*/
    }
}