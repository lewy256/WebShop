namespace ProductApi.Configurations;

public class KeyVaultConfiguration {
    public string Section { get; set; } = "KeyVault";
    public string KeyVaultUri { get; set; }
    public string SecretName { get; set; }
}
