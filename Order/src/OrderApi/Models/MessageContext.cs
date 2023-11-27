using Microsoft.EntityFrameworkCore;

namespace OrderApi.Models;

public class MessageContext : DbContext {
    private readonly string _connectionString;

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderItem> OrderItem { get; set; }

    public MessageContext(string connectionString) {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new Configurations.OrderConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderItemConfiguration());


        base.OnModelCreating(modelBuilder);
    }
}
