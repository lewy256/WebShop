namespace ProductApi.Shared.ReviewDtos;

public record ReviewDto {
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Description { get; init; }
    public int Rating { get; init; }
    public DateTime ReviewDate { get; init; }
}