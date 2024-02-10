namespace OrderApi.Consumers.Messages;

public class OrderDeleted {
    public string Name { get; set; } = "OrderDeleted";
    public List<Guid> ProductIds { get; set; }
}
