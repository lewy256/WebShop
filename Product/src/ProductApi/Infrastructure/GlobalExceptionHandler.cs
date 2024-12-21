using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ProductApi.Infrastructure;

public sealed class GlobalExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) {

        Log.Error(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
