namespace ProductApi.Entities;

public class WishlistItem {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ProductDetails Details { get; set; }
}
