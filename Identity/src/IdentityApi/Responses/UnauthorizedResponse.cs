namespace IdentityApi.Responses;

public class UnauthorizedResponse : ApiBaseResponse {
    public UnauthorizedResponse()
        : base(StatusCodes.Status401Unauthorized,
            "Authentication failed. Wrong user name or password.") {
    }
}
