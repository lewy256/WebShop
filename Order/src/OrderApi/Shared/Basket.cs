namespace OrderApi.Shared;

public class Basket {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<BasketItem> Items { get; set; }
    public decimal TotalPrice { get; set; }
}