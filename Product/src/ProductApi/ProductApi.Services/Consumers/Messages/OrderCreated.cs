namespace ProductApi.Service.Consumers.Messages;

public class OrderCreated {
    public string Name { get; init; } = "OrderCreated";
    public Guid BasketId { get; set; }
    public List<OrderPayload> Orders { get; set; }
}
