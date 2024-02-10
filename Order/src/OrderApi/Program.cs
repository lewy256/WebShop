using Carter;
using FluentValidation;
using HealthChecks.UI.Client;
using MicroElements.NSwag.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using OrderApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(builder.Configuration.GetValue<string>("AppConfig"))
        .Select(KeyFilter.Any, nameof(OrderApi) + builder.Environment.EnvironmentName);
});

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.ConfigureCors();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHealthCheck(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDbContext(builder.Configuration);

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.AddCarter();

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureJWT(builder.Configuration);

var app = builder.Build();

if(!app.Environment.IsDevelopment()) {
    app.UseCustomExceptionHandler();
}

if(app.Environment.IsProduction()) {
    app.UseHsts();
}

app.MapHealthChecks("/health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireHost();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
});

app.MapCarter();

app.UseHttpsRedirection();

app.Run();

public partial class Program {
}