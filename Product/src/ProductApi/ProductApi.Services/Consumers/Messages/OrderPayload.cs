namespace ProductApi.Service.Consumers.Messages;

public class OrderPayload {
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
