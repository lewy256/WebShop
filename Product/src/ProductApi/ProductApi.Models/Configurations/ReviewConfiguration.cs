using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Model.Entities;

namespace ProductApi.Model.Configurations;
public partial class ReviewConfiguration : IEntityTypeConfiguration<Review> {
    public void Configure(EntityTypeBuilder<Review> entity) {
        entity.ToContainer("ReviewsAndProductPrices")
            .HasPartitionKey(p => p.ProductId)
            .HasDiscriminator(r => r.Discriminator);

        entity.Property(p => p.Id)
            .ToJsonProperty("id")
            .HasConversion<string>();

        entity.Property(p => p.ProductId)
            .ToJsonProperty("productId")
            .HasConversion<string>();

        entity.Property(p => p.Description)
            .ToJsonProperty("description");

        entity.Property(p => p.Rating)
            .ToJsonProperty("rating");

        entity.Property(p => p.ReviewDate)
            .ToJsonProperty("reviewDate");

        entity.Property(p => p.Discriminator)
            .ToJsonProperty("discriminator");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Review> entity);
}