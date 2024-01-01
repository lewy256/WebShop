namespace ProductApi.Shared.Model.ReviewDtos;

public record UpdateReviewDto {
    public string Description { get; set; }
    public int Rating { get; set; }
}