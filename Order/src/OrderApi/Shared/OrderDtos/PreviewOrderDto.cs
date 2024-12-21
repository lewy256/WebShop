namespace OrderApi.Shared.OrderDtos;

public class PreviewOrderDto {
    public string CouponCode { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public decimal TotalPrice { get; set; }

}
