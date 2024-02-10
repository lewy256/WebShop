namespace IdentityApi.Responses;

public class RefreshTokenBadRequestResponse : ApiBaseResponse {
    public RefreshTokenBadRequestResponse()
     : base(StatusCodes.Status400BadRequest,
         "Invalid client request. The tokenDto has some invalid values.") {
    }
}
