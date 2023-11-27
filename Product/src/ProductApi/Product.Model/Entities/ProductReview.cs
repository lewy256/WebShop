namespace ProductApi.Model.Entities;

public class ProductReview {
    public int ProductReviewID { get; set; }
    public string ProductID { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public string Discriminator { get; set; }
    public string Id { get; set; }
}