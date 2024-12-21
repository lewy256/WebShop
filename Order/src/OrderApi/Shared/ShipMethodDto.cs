namespace OrderApi.Shared;

public record ShipMethodDto {
    public int Id { get; set; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
