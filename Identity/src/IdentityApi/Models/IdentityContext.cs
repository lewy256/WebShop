using IdentityApi.Models.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Models;

public partial class IdentitytContext : IdentityDbContext<User> {
    public IdentitytContext(DbContextOptions<IdentitytContext> options) : base(options) {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}
