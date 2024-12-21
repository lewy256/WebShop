using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Entities;

namespace OrderApi.Infrastructure.Configurations.Database;
public partial class SpecOrderStatusConfiguration : IEntityTypeConfiguration<SpecOrderStatus>
{
    public void Configure(EntityTypeBuilder<SpecOrderStatus> entity)
    {
        entity.Property(e => e.SpecOrderStatusId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.OrderId);

        entity.Property(e => e.StatusDate);

        entity.Property(e => e.StatusId);

        entity.HasOne(d => d.Order)
            .WithMany(p => p.SpecOrderStatus)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.Status)
            .WithMany(p => p.SpecOrderStatus)
            .HasForeignKey(d => d.StatusId);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<SpecOrderStatus> entity);
}
