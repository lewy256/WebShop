using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;

public partial class ShipMethodConfiguration : IEntityTypeConfiguration<ShipMethod>
{
    public void Configure(EntityTypeBuilder<ShipMethod> entity)
    {
        entity.Property(e => e.ShipMethodId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name);

        entity.Property(e => e.Price);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ShipMethod> entity);
}
