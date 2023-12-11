using HealthChecks.UI.Client;
using IdentityApi.Endpoints;
using IdentityApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

builder.Services.ConfigureHealthCheck(builder.Configuration);

builder.Services.ConfigureCors();

builder.Services.ConfigureDbContext(builder.Configuration);

builder.Services.ConfigureServices();

builder.Services.ConfigureIdentity();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");


app.MapIdentityEndpoints();
app.MapTokenEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireHost();

app.UseHttpsRedirection();


app.Run();

