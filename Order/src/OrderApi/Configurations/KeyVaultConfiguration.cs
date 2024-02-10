namespace OrderApi.Configurations;

public class KeyVaultConfiguration {
    public const string Section = "KeyVault";
    public string KeyVaultUri { get; init; }
    public string SecretName { get; init; }
}
