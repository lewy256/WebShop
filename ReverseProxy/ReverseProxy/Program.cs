using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(opt => {
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress!.ToString(),
        partition => new FixedWindowRateLimiterOptions {
            AutoReplenishment = true,
            PermitLimit = 30,
            QueueLimit = 2,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            Window = TimeSpan.FromMinutes(1)
        }));

    opt.AddPolicy("fixed", context =>
        RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress!.ToString(),
        partition => new FixedWindowRateLimiterOptions {
            AutoReplenishment = true,
            PermitLimit = 3,
            Window = TimeSpan.FromSeconds(10)
        }));

    opt.OnRejected = async (context, token) => {
        context.HttpContext.Response.StatusCode = 429;

        if(context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)) {
            await context.HttpContext.Response
                .WriteAsync($"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s).", token);
        }
        else {
            await context.HttpContext.Response
                .WriteAsync("Too many requests. Please try again later.", token);
        }
    };
});

var app = builder.Build();
app.UseRateLimiter();
app.MapReverseProxy();
app.Run();
