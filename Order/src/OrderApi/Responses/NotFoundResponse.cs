using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Responses;

public class NotFoundResponse : ProblemDetails {
    public NotFoundResponse(string id, string entity) {
        Detail = $"The {entity} with id: {id} doesn't exist in the database.";
        Status = StatusCodes.Status404NotFound;
    }

    public NotFoundResponse(string entity) {
        Detail = $"The {entity} doesn't exist in the database.";
        Status = StatusCodes.Status404NotFound;
    }
}
