namespace ProductApi.Model.LinkModels.PriceHistory;
public record LinkedPricesHistory {
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceValue { get; set; }
    public List<Link> Links { get; set; }
}
