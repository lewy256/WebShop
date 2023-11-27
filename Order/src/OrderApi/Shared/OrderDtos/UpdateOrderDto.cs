namespace OrderApi.Shared.OrderDtos;

public record UpdateOrderDto {
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public int SpecOrderStatusId { get; set; }
    public int PaymentMethodId { get; set; }
    public int AddressId { get; set; }
    public int ShipMethodId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Notes { get; set; }
    public int? CouponId { get; set; }
}