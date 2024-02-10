namespace OrderApi.Contracts;

public sealed record PaymentMethodRequest {
    public string Name { get; init; }
}
