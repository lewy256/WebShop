using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;

public partial class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem> {
    public void Configure(EntityTypeBuilder<OrderItem> entity) {
        entity.Property(e => e.OrderItemId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.OrderId);

        entity.Property(e => e.UnitPrice);

        entity.Property(e => e.ProductId);

        entity.HasOne(d => d.Order)
            .WithMany(p => p.OrderItem)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<OrderItem> entity);
}
