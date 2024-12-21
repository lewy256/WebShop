namespace OrderApi.Shared;

public record PaymentMethodDto {
    public int Id { get; set; }
    public string Name { get; init; }
}
