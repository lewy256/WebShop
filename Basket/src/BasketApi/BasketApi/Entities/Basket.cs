namespace BasketApi.Entities;

public class Basket {
    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new();

    public decimal TotalPrice {
        get {
            decimal totalprice = 0;
            foreach(var item in Items) {
                totalprice += item.Price * item.Quantity;
            }
            return totalprice;
        }
    }
}