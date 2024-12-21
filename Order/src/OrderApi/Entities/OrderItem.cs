namespace OrderApi.Entities;

public class OrderItem {
    public int OrderItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public virtual Order Order { get; set; }
}