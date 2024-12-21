namespace OrderApi.Entities;
public class ShipMethod {
    public ShipMethod() {
        Order = new HashSet<Order>();
    }

    public int ShipMethodId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public virtual ICollection<Order> Order { get; set; }
}