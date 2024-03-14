using BasketApi.Models;

namespace BasketApi.Consumers.Messages;

public class BasketCreated {
    public string Name { get; init; } = "BasketCreated";
    public Basket Basket { get; set; }
}
