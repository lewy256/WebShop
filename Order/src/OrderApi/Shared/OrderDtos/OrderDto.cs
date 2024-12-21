namespace OrderApi.Shared.OrderDtos;

public record OrderDto {
    public int Id { get; set; }
    public Guid OrderName { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
}
