using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Filters;

//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class MultipartFormDataAttribute : IActionFilter {
    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context) {
        var request = context.HttpContext.Request;

        if(request.HasFormContentType && request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase)) {
            return;
        }
        context.Result = new ObjectResult(new UnsupportedMediaTypeResponse()) { StatusCode = StatusCodes.Status415UnsupportedMediaType };
    }
}
