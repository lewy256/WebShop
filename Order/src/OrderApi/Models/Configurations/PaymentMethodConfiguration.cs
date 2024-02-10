using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderApi.Models.Configurations;

public partial class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod> {
    public void Configure(EntityTypeBuilder<PaymentMethod> entity) {
        entity.Property(e => e.PaymentMethodId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name).HasMaxLength(50);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PaymentMethod> entity);
}
