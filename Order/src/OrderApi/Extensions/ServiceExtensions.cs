using Microsoft.EntityFrameworkCore;
using OrderApi.Consumer;
using OrderApi.Models;

namespace OrderApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });
    }

    public static void ConfigureServices(this IServiceCollection services) {
        services.AddHostedService<Receiver>();
        services.AddSingleton<Publisher>();
        services.AddSingleton<Process>();
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<OrderContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TestDatabase")));
    }
}