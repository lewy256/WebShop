namespace ProductApi.Shared.Model.ReviewDtos;

public record CreateReviewDto {
    public string Description { get; init; }
    public int Rating { get; init; }
}