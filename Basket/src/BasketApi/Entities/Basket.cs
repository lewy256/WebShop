namespace BasketApi.Entities;

public class Basket {
    public string Id { get; set; }
    public List<BasketItem> Items { get; set; }
    public decimal TotalPrice { get; set; }
}