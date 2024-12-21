namespace OrderApi.Shared;

public record CouponDto {
    public string Code { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int MaxUsage { get; set; }
    public int UsedCount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; }
}
