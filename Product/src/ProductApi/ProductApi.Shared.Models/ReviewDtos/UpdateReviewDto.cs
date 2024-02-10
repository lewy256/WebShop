namespace ProductApi.Shared.Model.ReviewDtos;

public record UpdateReviewDto {
    public string Description { get; init; }
    public int Rating { get; init; }
}