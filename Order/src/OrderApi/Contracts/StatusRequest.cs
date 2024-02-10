namespace OrderApi.Contracts;

public sealed record StatusRequest {
    public string Description { get; init; }
}
