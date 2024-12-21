using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Shared.Responses;
public class UnsupportedMediaTypeResponse : ProblemDetails {
    public UnsupportedMediaTypeResponse() {
        Status = StatusCodes.Status415UnsupportedMediaType;
        Detail = "The server cannot process the media format of the requested resource.";
    }
}
