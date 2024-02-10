namespace OrderApi.Contracts;

public sealed record CouponRequest {
    public string Code { get; init; }
    public string Description { get; init; }
    public int Amount { get; init; }
}
