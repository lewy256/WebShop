using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.ShipMethods;

public static class GetShipMethod {
    public sealed record Query(int Id) : IRequest<ShipMethodGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, ShipMethodGetResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<ShipMethodGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var shipMethodDto = await _context.ShipMethod.AsNoTracking().ProjectToType<ShipMethodDto>().SingleOrDefaultAsync(o => o.ShipMethodId == request.Id);

            if(shipMethodDto is null) {
                return new NotFoundResponse(request.Id, nameof(ShipMethod));
            }

            return shipMethodDto;
        }
    }
}

public class GetShipMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/ship-methods/{id}",
          [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(ShipMethodDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetShipMethod.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetShipMethod)).WithTags(nameof(ShipMethod));
    }
}

[GenerateOneOf]
public partial class ShipMethodGetResponse : OneOfBase<ShipMethodDto, NotFoundResponse> {
}