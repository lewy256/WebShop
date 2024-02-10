namespace OrderApi.Shared;

public record StatusDto {
    public int StatusId { get; init; }
    public string Description { get; init; }
}
