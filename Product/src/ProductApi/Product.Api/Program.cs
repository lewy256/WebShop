using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Extensions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());


builder.Services.ConfigureCors();

builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureRateLimitingOptions();

builder.Services.ConfigureServices();

builder.Services.AddValidatorsFromAssembly(Assembly.Load("ProductApi.Services"));

builder.Services.ConfigureCosmosDB(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddAuthentication();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddCustomMediaTypes();

builder.Services.ConfigureVersioning();

builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.ConfigureOutputCaching(builder.Configuration);

var app = builder.Build();


if(app.Environment.IsProduction()) {
    app.UseHsts();
}


app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.All
});

if(app.Environment.IsProduction()) {
    app.UseRateLimiter();
}

app.UseCors("CorsPolicy");

if(app.Environment.IsProduction()) {
    app.UseOutputCache();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Prodcut API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Test API v2");

});

app.MapControllers();

app.Run();


public partial class Program {
}