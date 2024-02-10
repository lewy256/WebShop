﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderApi.Models.Configurations;
public partial class CouponConfiguration : IEntityTypeConfiguration<Coupon> {
    public void Configure(EntityTypeBuilder<Coupon> entity) {
        entity.Property(e => e.CouponId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(e => e.Description).HasMaxLength(50);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Coupon> entity);
}
