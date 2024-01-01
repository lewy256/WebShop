namespace ProductApi.Shared.Model.ReviewDtos;

public record ReviewDto {
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
}