namespace OrderApi.Shared;

public record OrderSummaryDto {
    public int OrderId { get; init; }
    public Guid OrderName { get; init; }
    public DateTime OrderDate { get; init; }
    public PaymentMethodDto PaymentMethod { get; init; }
    public AddressDto Address { get; init; }
    public ShipMethodDto ShipMethod { get; init; }
    public decimal TotalPrice { get; init; }
    public string Notes { get; init; }
    public CouponDto Coupon { get; init; }
    public List<OrderItemDto> OrderItems { get; init; } = new List<OrderItemDto>();
    public List<StatusDto> Statuses { get; init; } = new List<StatusDto>();
}
