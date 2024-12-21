using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;
public partial class StatusConfiguration : IEntityTypeConfiguration<Status> {
    public void Configure(EntityTypeBuilder<Status> entity) {
        entity.Property(e => e.StatusId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Description)
            .HasMaxLength(10);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Status> entity);
}
