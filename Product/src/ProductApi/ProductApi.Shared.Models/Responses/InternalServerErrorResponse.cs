using Microsoft.AspNetCore.Http;

namespace ProductApi.Shared.Model.Responses;

public class InternalServerErrorResponse : ApiBaseResponse {
    public InternalServerErrorResponse(string message)
        : base(StatusCodes.Status500InternalServerError, message) {
    }
}
