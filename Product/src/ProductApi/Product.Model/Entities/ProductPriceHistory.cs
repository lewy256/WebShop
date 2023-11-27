namespace ProductApi.Model.Entities;

public class ProductPriceHistory {
    public string Id { get; set; }
    public int PriceHistoryID { get; set; }
    public string ProductID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float Price { get; set; }
    public string Discriminator { get; set; }
}