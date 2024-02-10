using Microsoft.AspNetCore.Http;

namespace ProductApi.Shared.Model.Responses;

public class BadRequestResponse : ApiBaseResponse {
    public BadRequestResponse(string message)
        : base(StatusCodes.Status400BadRequest, message) {
    }
}
