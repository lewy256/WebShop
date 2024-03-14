using BasketApi.Endpoints;
using BasketApi.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(builder.Configuration.GetValue<string>("AzureConfiguration"))
        .Select(KeyFilter.Any, nameof(BasketApi) + builder.Environment.EnvironmentName);
});

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureCors();

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.ConfigureServices();

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureJWT(builder.Configuration);

var app = builder.Build();

app.UseCustomExceptionHandler();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
});

app.MapBasketEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireHost();

app.UseHttpsRedirection();

app.Run();

public partial class Program {
}