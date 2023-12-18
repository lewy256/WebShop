namespace ProductApi.Model.Entities;

public class Product {
    public string ProductName { get; set; }
    public int SerialNumber { get; set; }
    public string ImageName { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public long Weight { get; set; }
    public string Size { get; set; }
    public Guid Id { get; set; }
}
