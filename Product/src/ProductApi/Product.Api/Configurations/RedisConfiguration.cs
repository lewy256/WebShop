namespace ProductApi.Configurations;

public class RedisConfiguration {
    public string Section { get; set; } = "Redis";
    public string RedisCache { get; set; }
    public string InstanceName { get; set; }
}
