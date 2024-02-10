namespace OrderApi.Contracts;

public class OrderRequest {
    public int PaymentMethodId { get; set; }
    public int AddressId { get; set; }
    public int ShipMethodId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public int? CouponId { get; set; }
}
