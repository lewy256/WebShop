using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;

public partial class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> entity)
    {
        entity.Property(e => e.PaymentMethodId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(50);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PaymentMethod> entity);
}
