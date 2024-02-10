namespace OrderApi.Shared;

public record PaymentMethodDto {
    public int PaymentMethodId { get; init; }
    public string Name { get; init; }
}
