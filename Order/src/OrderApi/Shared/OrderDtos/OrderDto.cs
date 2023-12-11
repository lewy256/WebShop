namespace OrderApi.Shared.OrderDtos;

public record OrderDto {
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public int PaymentMethodId { get; set; }
    public int AddressId { get; set; }
    public int ShipMethodId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public int? CouponId { get; set; }
    public Guid RowKey { get; set; }

}