namespace ProductApi.Shared.Model.ReviewDtos;

public record CreateReviewDto {
    public Guid ProductId { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
}