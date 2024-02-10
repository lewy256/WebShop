namespace ProductApi.Shared.Model;
public sealed record ValidationError {
    public string PropertyName { get; init; }
    public string ErrorMessage { get; init; }
}

