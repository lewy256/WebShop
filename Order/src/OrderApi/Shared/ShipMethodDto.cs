namespace OrderApi.Shared;

public record ShipMethodDto {
    public int ShipMethodId { get; init; }
    public string Description { get; init; }
    public DateTime DeliveryTime { get; init; }
    public decimal Price { get; init; }
}
