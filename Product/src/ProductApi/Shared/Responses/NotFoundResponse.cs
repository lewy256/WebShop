using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Shared.Responses;

public class NotFoundResponse : ProblemDetails {
    public NotFoundResponse(Guid id, string entity) {
        Detail = $"The {entity} with id: {id} doesn't exist in the database.";
        Status = StatusCodes.Status404NotFound;
    }

    public NotFoundResponse(string message) {
        Detail = message;
        Status = StatusCodes.Status404NotFound;
    }
}
