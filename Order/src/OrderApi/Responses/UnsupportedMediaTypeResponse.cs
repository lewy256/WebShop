namespace OrderApi.Responses;
public class UnsupportedMediaTypeResponse : ApiBaseResponse {
    public UnsupportedMediaTypeResponse()
        : base(StatusCodes.Status415UnsupportedMediaType,
            "The server cannot process the media format of the requested resource.") {
    }
}
