using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderApi.Models.Configurations;
public partial class OrderConfiguration : IEntityTypeConfiguration<Order> {
    public void Configure(EntityTypeBuilder<Order> entity) {
        entity.Property(e => e.OrderId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.AddressId);

        entity.Property(e => e.CouponId);

        entity.Property(e => e.CustomerId);

        entity.Property(e => e.Notes);

        entity.Property(e => e.OrderDate);

        entity.Property(e => e.PaymentMethodId);

        entity.Property(e => e.ShipMethodId);

        entity.Property(e => e.TotalPrice);

        entity.Property(e => e.OrderName);

        entity.HasOne(d => d.Address)
            .WithMany(p => p.Order)
            .HasForeignKey(d => d.AddressId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(d => d.Coupon)
            .WithMany(p => p.Order)
            .HasForeignKey(d => d.CouponId);

        entity.HasOne(d => d.PaymentMethod)
            .WithMany(p => p.Order)
            .HasForeignKey(d => d.PaymentMethodId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(d => d.ShipMethod)
            .WithMany(p => p.Order)
            .HasForeignKey(d => d.ShipMethodId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Order> entity);
}
