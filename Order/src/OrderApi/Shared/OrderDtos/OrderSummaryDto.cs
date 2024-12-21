using OrderApi.Shared.AddressDtos;

namespace OrderApi.Shared.OrderDtos;

public record OrderSummaryDto {
    public Guid OrderName { get; init; }
    public DateTime OrderDate { get; init; }
    public PaymentMethodDto PaymentMethod { get; init; }
    public AddressDto Address { get; init; }
    public ShipMethodDto ShipMethod { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalPrice { get; init; }
    public string? Notes { get; init; }
    public CouponDto? Coupon { get; init; }
    public List<OrderItemDto> OrderItems { get; init; } = new List<OrderItemDto>();
    public List<StatusDto> Statuses { get; init; } = new List<StatusDto>();
}
