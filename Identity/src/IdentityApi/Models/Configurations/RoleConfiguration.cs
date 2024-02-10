using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityApi.Models.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole> {
    public void Configure(EntityTypeBuilder<IdentityRole> builder) {
        builder.HasData(
          new IdentityRole {
              Name = "Customer",
              NormalizedName = "CUSTOMER"
          },
          new IdentityRole {
              Name = "Administrator",
              NormalizedName = "ADMINISTRATOR"
          }
        );
    }
}
