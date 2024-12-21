namespace ProductApi.Entities;

public class ProductDetails {
    public Guid ProductId { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Brand { get; init; }
    public string? Image { get; init; }
}
