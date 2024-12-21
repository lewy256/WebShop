using ProductApi.Shared.ReviewDtos;

namespace ProductApi.Shared.LinkModels.Reviews;
public record LinkedReviews {
    public Guid Id { get; set; }
    public string UserName { get; init; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewDate { get; init; }
    public List<Link> Links { get; init; }

    public LinkedReviews(ReviewDto review, List<Link> links) {
        Id = review.Id;
        UserName = review.UserName;
        Description = review.Description;
        Rating = review.Rating;
        ReviewDate = review.ReviewDate;
        Links = links;

    }

    public LinkedReviews() { }
}
