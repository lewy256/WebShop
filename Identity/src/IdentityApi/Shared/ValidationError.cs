namespace IdentityApi.Shared;
public record ValidationError {
    public string PropertyName { get; init; }
    public string ErrorMessage { get; init; }
}

