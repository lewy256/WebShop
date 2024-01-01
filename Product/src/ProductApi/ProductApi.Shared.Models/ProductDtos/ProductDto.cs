namespace ProductApi.Shared.Model.ProductDtos;

public record ProductDto {
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public string SerialNumber { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public int Weight { get; set; }
    public string Size { get; set; }
}