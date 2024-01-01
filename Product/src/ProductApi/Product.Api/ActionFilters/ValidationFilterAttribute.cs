using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductApi.ActionFilters;

public class ValidationFilterAttribute : IActionFilter {
    public ValidationFilterAttribute() {
    }

    public void OnActionExecuting(ActionExecutingContext context) {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var param = context.ActionArguments
            .SingleOrDefault(x => x.Value.ToString().Contains("Dto")).Value;
        if(param is null) {
            context.Result = new BadRequestObjectResult($"Object is null. Controller:{controller}, action: {action}");
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) {
    }
}