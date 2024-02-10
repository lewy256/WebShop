namespace ProductApi.Configurations;

public class JwtConfiguration {
    public const string Section = "JwtSettings";
    public string? ValidIssuer { get; init; }
    public string? ValidAudience { get; init; }
    public string? Expires { get; init; }
}
