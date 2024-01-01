using Carter;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using OrderApi.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers(options => {
//    options.ReturnHttpNotAcceptable = true;
//    options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
//});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

builder.Services.ConfigureCors();

//builder.Services.ConfigureServices();

//builder.Services.AddAuthentication();

builder.Services.ConfigureDbContext(builder.Configuration);


//builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction()) {
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");


//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.MigrateDatabase();

app.MapCarter();

app.UseHttpsRedirection();

app.Run();


NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() {
    return new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();
}

public partial class Program {
}