using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderApi.Models.Configurations;

public partial class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem> {
    public void Configure(EntityTypeBuilder<OrderItem> entity) {
        entity.Property(e => e.OrderItemId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.OrderId);

        entity.Property(e => e.Price);

        entity.Property(e => e.ProductId);

        entity.HasOne(d => d.Order)
            .WithMany(p => p.OrderItem)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<OrderItem> entity);
}
