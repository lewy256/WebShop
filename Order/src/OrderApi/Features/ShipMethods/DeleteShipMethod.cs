using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.ShipMethods;

public static class DeleteShipMethod {
    public sealed record Command(int Id) : IRequest<ShipMethodDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, ShipMethodDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<ShipMethodDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            int rows = await _context.ShipMethod.Where(p => p.ShipMethodId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(ShipMethod));
            }

            return new Success();
        }
    }
}

public class DeleteShipMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/ship-methods/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteShipMethod.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteShipMethod)).WithTags(nameof(ShipMethod));
    }
}

[GenerateOneOf]
public partial class ShipMethodDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}