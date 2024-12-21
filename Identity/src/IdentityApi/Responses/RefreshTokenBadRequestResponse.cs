using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Responses;

public class RefreshTokenBadRequestResponse : ProblemDetails {
    public RefreshTokenBadRequestResponse() {
        Detail = "Invalid client request. The tokenDto has some invalid values.";
        Status = StatusCodes.Status400BadRequest;
    }
}
