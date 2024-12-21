namespace Contracts.Messages;

public class OrderDeleted {
    public required Dictionary<Guid, int> Products { get; set; }
}
