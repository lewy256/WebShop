using Microsoft.AspNetCore.Http;

namespace ProductApi.Shared.Model.Responses;
public class BlobNotFoundResponse : ApiBaseResponse {
    public BlobNotFoundResponse()
        : base(StatusCodes.Status404NotFound, "Image does not exist.") {
    }
}
