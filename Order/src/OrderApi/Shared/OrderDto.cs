namespace OrderApi.Shared;

public record OrderDto {
    public int OrderId { get; init; }
    public Guid OrderName { get; set; }
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
}
