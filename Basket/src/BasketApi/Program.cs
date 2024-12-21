using BasketApi.Endpoints;
using BasketApi.Extensions;
using BasketApi.Infrastructure;
using FluentValidation;
using HealthChecks.UI.Client;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(builder.Configuration.GetValue<string>("AZURE_APP_CONFIGURATION"))
        .Select(KeyFilter.Any, nameof(BasketApi) + builder.Environment.EnvironmentName);
});

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureCors();

builder.Services.ConfigureAuthorization();

builder.Services.ConfigureRedis(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.ConfigureServices();

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

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