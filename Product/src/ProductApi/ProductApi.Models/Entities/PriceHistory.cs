namespace ProductApi.Model.Entities;

public class PriceHistory {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceValue { get; set; }
    public string Discriminator { get; set; }
}