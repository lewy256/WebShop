using OrderApi.Shared;

namespace OrderApi.Consumers.Messages;

public class BasketCreated {
    public string Name { get; init; } = "BasketCreated";
    public Basket Basket { get; set; }
}