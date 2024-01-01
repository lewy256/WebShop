namespace ProductApi.Shared.Model.ReviewDtos;

public record CreateReviewDto {
    public string Description { get; set; }
    public int Rating { get; set; }
}