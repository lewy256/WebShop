using FluentValidation;
using HealthChecks.UI.Client;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using ProductApi.Extensions;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => {
    options.Connect(builder.Configuration.GetValue<string>("AZURE_APP_CONFIGURATION"))
        .Select(KeyFilter.Any, nameof(ProductApi) + builder.Environment.EnvironmentName);
});

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.ConfigureCors();
builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; });
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureProblemDetails();

builder.Services.ConfigureAuthorization();

builder.Services.ConfigureMassTransit(builder.Configuration);

builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Services.ConfigureServices();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.ConfigureCosmosDB(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddCustomMediaTypes();

builder.Services.ConfigureVersioning();

builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.ConfigureOutputCaching(builder.Configuration);

builder.Services.AddScoped<MultipartFormDataAttribute>();

builder.Services.ConfigureAzureStorage(builder.Configuration);


var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

if(app.Environment.IsProduction()) {
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.All
});

if(app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.MapHealthChecks("/health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireHost();

app.UseCors("CorsPolicy");



if(app.Environment.IsProduction()) {
    app.UseOutputCache();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Prodcut API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Product API v2");

});

app.MapControllers();

app.Run();

public partial class Program {
}

