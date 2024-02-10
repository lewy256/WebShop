namespace OrderApi.Responses;

public class InternalServerErrorResponse : ApiBaseResponse {
    public InternalServerErrorResponse(string message)
        : base(StatusCodes.Status500InternalServerError, message) {
    }
}
