namespace OrderApi.Shared;

public record OrderItemDto {
    public int OrderItemId { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public Guid ProductId { get; init; }
}
