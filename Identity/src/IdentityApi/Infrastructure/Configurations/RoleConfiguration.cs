using Contracts.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityApi.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole> {
    public void Configure(EntityTypeBuilder<IdentityRole> builder) {
        builder.HasData(
          new IdentityRole {
              Name = UserRole.Customer,
              NormalizedName = UserRole.Customer.ToUpper(),
          },
          new IdentityRole {
              Name = UserRole.Administrator,
              NormalizedName = UserRole.Administrator.ToUpper(),
          }
        );
    }
}
