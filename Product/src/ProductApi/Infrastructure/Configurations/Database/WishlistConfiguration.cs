using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Entities;

namespace ProductApi.Infrastructure.Configurations.Database;

public partial class WishlistConfiguration : IEntityTypeConfiguration<WishlistItem> {
    public void Configure(EntityTypeBuilder<WishlistItem> entity) {
        entity.ToContainer("Wishlist")
            .HasPartitionKey(p => p.UserId)
            .HasNoDiscriminator();

        entity.OwnsOne(w => w.Details, details => {
            details.ToJsonProperty("productDetails");
            details.Property(f => f.ProductId).ToJsonProperty("productId");
            details.Property(f => f.ProductName).ToJsonProperty("productName");
            details.Property(f => f.Price).ToJsonProperty("price");
            details.Property(f => f.Stock).ToJsonProperty("stock");
            details.Property(f => f.Brand).ToJsonProperty("brand");
            details.Property(f => f.Image).ToJsonProperty("image");
        });

        entity.Property(p => p.UserId)
            .ToJsonProperty("userId")
            .HasConversion<string>();

        entity.Property(x => x.Id)
            .ToJsonProperty("id")
            .HasConversion<string>();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<WishlistItem> entity);
}