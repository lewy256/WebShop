namespace ProductApi.Shared.Model.ProductDtos;

public record CreateProductDto {
    public string ProductName { get; init; }
    public string SerialNumber { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Description { get; init; }
    public string Color { get; init; }
    public int Weight { get; init; }
    public string Size { get; init; }
}