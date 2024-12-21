using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Responses;

public class UnauthorizedResponse : ProblemDetails {
    public UnauthorizedResponse() {
        Detail = "Authentication failed. Wrong user name or password.";
        Status = StatusCodes.Status401Unauthorized;
    }
}
