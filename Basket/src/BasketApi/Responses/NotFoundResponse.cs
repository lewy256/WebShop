using Microsoft.AspNetCore.Mvc;

namespace BasketApi.Responses;
public class NotFoundResponse : ProblemDetails {
    public NotFoundResponse(string entity) {
        Detail = $"The {entity} doesn't exist in the database.";
        Status = StatusCodes.Status404NotFound;
    }
}
