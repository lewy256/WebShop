namespace ProductApi.Infrastructure.Configurations.AppSettings;

public class JwtConfiguration {
    public const string Section = "JwtSettings";
    public string ValidIssuer { get; init; }
    public string ValidAudience { get; init; }
    public string SecretName { get; init; }
    public string KeyVaultUri { get; init; }
}
