namespace OrderApi.Shared;

public class Basket {
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }

}