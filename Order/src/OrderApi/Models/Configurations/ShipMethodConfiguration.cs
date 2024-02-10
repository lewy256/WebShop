using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderApi.Models.Configurations;

public partial class ShipMethodConfiguration : IEntityTypeConfiguration<ShipMethod> {
    public void Configure(EntityTypeBuilder<ShipMethod> entity) {
        entity.Property(e => e.ShipMethodId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.DeliveryTime);

        entity.Property(e => e.Description)
            .HasMaxLength(10);

        entity.Property(e => e.Price);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ShipMethod> entity);
}
