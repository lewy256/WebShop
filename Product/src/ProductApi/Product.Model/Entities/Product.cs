namespace ProductApi.Model.Entities;

public class Product {
    //    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }
    public int ProductID { get; set; }
    public string Name { get; set; }
    public int ProductNumber { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public int CategoryID { get; set; }
    public string Category { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public long? Weight { get; set; }
}