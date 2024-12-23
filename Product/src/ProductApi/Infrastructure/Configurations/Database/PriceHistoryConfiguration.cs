﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Entities;

namespace ProductApi.Infrastructure.Configurations.Database;
public partial class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> entity)
    {
        entity.ToContainer("ReviewsAndProductPrices")
            .HasPartitionKey(p => p.ProductId)
            .HasDiscriminator(p => p.Discriminator);

        entity.Property(x => x.Id)
            .ToJsonProperty("id")
            .HasConversion<string>();

        entity.Property(p => p.ProductId)
            .ToJsonProperty("productId")
            .HasConversion<string>();

        entity.Property(p => p.StartDate)
            .ToJsonProperty("startDate")
            .HasConversion<string>();

        entity.Property(p => p.EndDate)
            .ToJsonProperty("endDate");

        entity.Property(p => p.PriceValue)
            .ToJsonProperty("price");

        entity.Property(p => p.Discriminator)
            .ToJsonProperty("discriminator");


        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PriceHistory> entity);
}