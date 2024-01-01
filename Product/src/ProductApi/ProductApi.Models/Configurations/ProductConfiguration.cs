using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Model.Entities;

namespace ProductApi.Model.Configurations {
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

            entity.Property(p => p.Color)
                .ToJsonProperty("color");

            entity.Property(p => p.Description)
                .ToJsonProperty("description");

            entity.Property(p => p.SerialNumber)
                .ToJsonProperty("serialNumber");

            entity.Property(p => p.Size)
                .ToJsonProperty("size");

            entity.Property(p => p.ProductName)
                .ToJsonProperty("productName");

            entity.Property(p => p.Weight)
                .ToJsonProperty("weight");

            entity.Property(x => x.Id)
                .ToJsonProperty("id")
                .HasConversion<string>();

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Product> entity);
    }
}
