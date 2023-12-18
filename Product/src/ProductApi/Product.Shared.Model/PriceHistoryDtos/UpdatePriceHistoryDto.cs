namespace ProductApi.Shared.Model.PriceDtos;

public record UpdatePriceHistoryDto {
    public string Name { get; set; }
    public int ProductNumber { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryID { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public long Weight { get; set; }
}