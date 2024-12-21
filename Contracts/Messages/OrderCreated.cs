namespace Contracts.Messages;

public class OrderCreated {
    public required string UserId { get; set; }
    public required Dictionary<Guid, int> Products { get; set; }
}
