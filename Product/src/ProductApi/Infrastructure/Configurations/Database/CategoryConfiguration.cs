﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Entities;

namespace ProductApi.Infrastructure.Configurations.Database;
public partial class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.ToContainer("Categories")
            .HasPartitionKey(p => p.Id)
            .HasNoDiscriminator();

        entity.Property(x => x.Id)
            .ToJsonProperty("id")
            .HasConversion<string>();

        entity.Property(p => p.CategoryName)
            .ToJsonProperty("categoryName");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Category> entity);
}