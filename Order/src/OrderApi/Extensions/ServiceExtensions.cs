using Microsoft.EntityFrameworkCore;
using OrderApi.Intefaces;
using OrderApi.Models;
using OrderApi.Services;

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
        services.AddScoped<IOrderService, OrderService>();
        services.AddHostedService<ReceiverService>();
        services.AddSingleton<PublisherService>();
        services.AddSingleton<ProcessService>();
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<OrderContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TestDatabase")));
    }
}