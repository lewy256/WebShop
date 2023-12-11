namespace ProductApi.Model.Entities;

public class ProductReview {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public string Discriminator { get; set; }
}