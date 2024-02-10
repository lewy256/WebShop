namespace OrderApi.Contracts;

public sealed record ShipMethodRequest {
    public string Description { get; init; }
    public DateTime DeliveryTime { get; init; }
    public decimal Price { get; init; }
}
