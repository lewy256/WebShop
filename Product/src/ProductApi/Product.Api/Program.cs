using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using ProductApi.ActionFilters;
using ProductApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logger = new LoggerConfiguration()
    //.ReadFrom.Configuration(builder.Configuration)
    // .Enrich.FromLogContext()
    .WriteTo.Console()
    //.WriteTo.AzureApp()
    .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());


builder.Services.ConfigureCors();

builder.Services.AddScoped<ValidationFilterAttribute>();


builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices();
builder.Services.ConfigureCosmosDB(builder.Configuration.GetSection("CosmosDb"));

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });


var app = builder.Build();

app.ConfigureExceptionHandler();


// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction()) {
    app.UseHsts();
}


app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program {
}