namespace OrderApi.Responses;

public class BadRequestResponse : ApiBaseResponse {
    public BadRequestResponse(string message = "Object is null.")
       : base(StatusCodes.Status400BadRequest, message) {
    }
}
