using FluentValidation;
using HealthChecks.UI.Client;
using IdentityApi.Endpoints;
using IdentityApi.Extensions;
using IdentityApi.Infrastructure;
using MicroElements.NSwag.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(builder.Configuration.GetValue<string>("AZURE_APP_CONFIGURATION"))
        .Select(KeyFilter.Any, nameof(IdentityApi) + builder.Environment.EnvironmentName);
});

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.ConfigureHealthCheck(builder.Configuration);

builder.Services.ConfigureCors();

builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureProblemDetails();

builder.Services.ConfigureDbContext(builder.Configuration);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.ConfigureServices();

builder.Services.ConfigureIdentity();

builder.Services.ConfigureSwagger();

builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1");
});

app.UseCors("CorsPolicy");

app.MapIdentityEndpoints();
app.MapTokenEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireHost();

app.UseHttpsRedirection();

app.MigrateDatabase();

app.Run();

public partial class Program { }

