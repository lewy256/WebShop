using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Entities;

namespace ProductApi.Infrastructure.Configurations.Database {
    public partial class ProductConfiguration : IEntityTypeConfiguration<Product> {
        public void Configure(EntityTypeBuilder<Product> entity) {
            entity.ToContainer("Products")
                .HasPartitionKey(p => p.CategoryId)
                .HasNoDiscriminator();

            entity.Property(p => p.CategoryId)
                .ToJsonProperty("categoryId")
                .HasConversion<string>();

            entity.Property(p => p.Stock)
                .ToJsonProperty("stock");

            entity.Property(p => p.Price)
                .ToJsonProperty("price");

            entity.Property(p => p.Colors)
                .ToJsonProperty("colors");

            entity.Property(p => p.Description)
                .ToJsonProperty("description");

            entity.Property(p => p.SerialNumber)
                .ToJsonProperty("serialNumber");

            entity.Property(p => p.Measurements)
                .ToJsonProperty("measurements");

            entity.Property(p => p.ProductName)
                .ToJsonProperty("productName");

            entity.Property(p => p.DispatchTime)
                .ToJsonProperty("dispatchTime");

            entity.Property(p => p.Weight)
                .ToJsonProperty("weight");

            entity.OwnsMany(p => p.Files, file => {
                file.ToJsonProperty("files");
                file.Property(f => f.Id).ToJsonProperty("id").HasConversion<string>();
                file.Property(f => f.URI).ToJsonProperty("uri");
            });

            entity.Property(x => x.Id)
                .ToJsonProperty("id")
                .HasConversion<string>();

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Product> entity);
    }
}
