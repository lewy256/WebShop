namespace ProductApi.Model.Entities;

public class ProductPriceHistory {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public string Discriminator { get; set; }
}