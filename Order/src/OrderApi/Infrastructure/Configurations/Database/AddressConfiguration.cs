using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;
public partial class AddressConfiguration : IEntityTypeConfiguration<Address> {
    public void Configure(EntityTypeBuilder<Address> entity) {
        entity.Property(e => e.AddressId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.AddressLine1).HasMaxLength(50);

        entity.Property(e => e.AddressLine2).HasMaxLength(50);

        entity.Property(e => e.City).HasMaxLength(50);

        entity.Property(e => e.Country).HasMaxLength(50);

        entity.Property(e => e.FirstName).HasMaxLength(50);

        entity.Property(e => e.LastName).HasMaxLength(50);

        entity.Property(e => e.PhoneNumber).HasMaxLength(50);

        entity.Property(e => e.PostalCode).HasMaxLength(50);

        entity.HasIndex(e => e.CustomerId);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Address> entity);
}
