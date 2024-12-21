namespace OrderApi.Entities;

public class Order {
    public Order() {
        OrderItem = new HashSet<OrderItem>();
        SpecOrderStatus = new HashSet<SpecOrderStatus>();
    }

    public int OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public int PaymentMethodId { get; set; }
    public int AddressId { get; set; }
    public int ShipMethodId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public int? CouponId { get; set; }
    public Guid OrderName { get; set; }

    public virtual Address Address { get; set; }
    public virtual Coupon Coupon { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }
    public virtual ShipMethod ShipMethod { get; set; }
    public virtual ICollection<OrderItem> OrderItem { get; set; }
    public virtual ICollection<SpecOrderStatus> SpecOrderStatus { get; set; }
}