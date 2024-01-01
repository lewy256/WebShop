namespace ProductApi.Model.LinkModels.Reviews;
public record LinkedReviews {
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public List<Link> Links { get; set; }
}
