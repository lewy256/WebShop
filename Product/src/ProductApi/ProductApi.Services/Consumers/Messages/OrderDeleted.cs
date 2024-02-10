namespace ProductApi.Service.Consumers.Messages;
public class OrderDeleted {
    public string Name { get; set; } = "OrderDeleted";
    public List<OrderPayload> Orders { get; set; }
}
