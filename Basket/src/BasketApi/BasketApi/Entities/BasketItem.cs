namespace BasketApi.Entities;

public class BasketItem {
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}