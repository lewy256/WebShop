namespace OrderApi.Entities;
public class Coupon {
    public Coupon() {
        Order = new HashSet<Order>();
    }

    public int CouponId { get; set; }
    public string Code { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int MaxUsage { get; set; }
    public int UsedCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Order> Order { get; set; }
}
