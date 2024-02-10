namespace OrderApi.Shared;

public record SpecOrderStatusDto {
    public int SpecOrderStatusId { get; init; }
    public int OrderId { get; init; }
    public int StatusId { get; init; }
    public DateTime StatusDate { get; init; }
}
