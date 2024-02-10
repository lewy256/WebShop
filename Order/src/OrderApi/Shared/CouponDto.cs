namespace OrderApi.Shared;

public record CouponDto {
    public int CouponId { get; init; }
    public string Code { get; init; }
    public string Description { get; init; }
    public int Amount { get; init; }
}
