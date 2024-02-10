namespace ProductApi.Shared.Model.ProductDtos;

public record ProductDto {
    public Guid Id { get; init; }
    public string ProductName { get; init; }
    public string SerialNumber { get; init; }
    public List<string> ImageUris { get; init; } = new List<string>();
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Description { get; init; }
    public string Color { get; init; }
    public int Weight { get; init; }
    public string Size { get; init; }
}