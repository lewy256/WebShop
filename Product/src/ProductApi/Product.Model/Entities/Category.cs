namespace ProductApi.Model.Entities;

public class Category {
    public Guid Id { get; set; }
    public int CategoryID { get; set; }
    public string Discriminator { get; set; }
    public string CategoryName { get; set; }
}