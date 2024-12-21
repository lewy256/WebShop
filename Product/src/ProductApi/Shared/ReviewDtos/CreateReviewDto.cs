namespace ProductApi.Shared.ReviewDtos;

public record CreateReviewDto {
    public string Description { get; init; }
    public int Rating { get; init; }
}