namespace BlobApi;

public static class BlobEndpoints {
    public static void MapBlobEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("api/blob");

    }
}
