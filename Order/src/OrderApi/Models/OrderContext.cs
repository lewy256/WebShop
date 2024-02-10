using Microsoft.EntityFrameworkCore;

namespace OrderApi.Models;
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
        modelBuilder.ApplyConfiguration(new Configurations.AddressConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CouponConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.PaymentMethodConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ShipMethodConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SpecOrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.StatusConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
