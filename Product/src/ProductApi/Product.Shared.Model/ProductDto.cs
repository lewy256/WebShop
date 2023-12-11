namespace ProductApi.Shared.Model;

public record ProductDto {
    public string Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}