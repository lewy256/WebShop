namespace OrderApi.Models;

public class Address {
    public Address() {
        Order = new HashSet<Order>();
    }

    public int AddressId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string PostalCode { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public Guid CustomerId { get; set; }

    public virtual ICollection<Order> Order { get; set; }
}