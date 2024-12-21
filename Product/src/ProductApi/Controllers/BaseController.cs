using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Controllers;

public abstract class BaseController : ControllerBase {
    [NonAction]
    public virtual ObjectResult Problem(ProblemDetails problemDetails) {
        var problemToReturn = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: problemDetails.Status ?? 500,
            title: problemDetails.Title,
            type: problemDetails.Type,
            detail: problemDetails.Detail,
            instance: problemDetails.Instance
        );

        foreach(var item in problemDetails.Extensions) {
            problemToReturn.Extensions[item.Key] = item.Value;
        }

        return new ObjectResult(problemToReturn) {
            StatusCode = problemToReturn.Status
        };
    }
}