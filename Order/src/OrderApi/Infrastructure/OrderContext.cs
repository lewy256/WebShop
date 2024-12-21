using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;
using OrderApi.Infrastructure.Configurations.Database;

namespace OrderApi.Infrastructure;
public partial class OrderContext : DbContext {
    public OrderContext(DbContextOptions<OrderContext> options)
        : base(options) { }

    public virtual DbSet<Address> Address { get; set; }
    public virtual DbSet<Coupon> Coupon { get; set; }
    public virtual DbSet<Order> Order { get; set; }
    public virtual DbSet<OrderItem> OrderItem { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
    public virtual DbSet<ShipMethod> ShipMethod { get; set; }
    public virtual DbSet<SpecOrderStatus> SpecOrderStatus { get; set; }
    public virtual DbSet<Status> Status { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new CouponConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
        modelBuilder.ApplyConfiguration(new ShipMethodConfiguration());
        modelBuilder.ApplyConfiguration(new SpecOrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new StatusConfiguration());

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
