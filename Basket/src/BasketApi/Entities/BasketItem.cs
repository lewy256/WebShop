namespace BasketApi.Entities;

public class BasketItem {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}