namespace OrderApi.Models;
public class Coupon {
    public Coupon() {
        Order = new HashSet<Order>();
    }

    public int CouponId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }

    public virtual ICollection<Order> Order { get; set; }
}