namespace OrderApi.Models;
public class PaymentMethod {
    public PaymentMethod() {
        Order = new HashSet<Order>();
    }

    public int PaymentMethodId { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Order> Order { get; set; }
}