using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared;

namespace OrderApi.Features.ShipMethods;

public static class GetShipMethods {
    public sealed record Query : IRequest<ShipMethodsGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, ShipMethodsGetAllResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<ShipMethodsGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var shipMethodDtos = await _context.ShipMethod.AsNoTracking().ProjectToType<ShipMethodDto>().ToListAsync();

            return new ShipMethodsGetAllResponse(shipMethodDtos);
        }
    }
}

public class GetShipMethodsEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/ship-methods",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(IEnumerable<ShipMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender) => {
            var query = new GetShipMethods.Query();

            var results = await sender.Send(query);

            return Results.Ok(results.ShipMethods);

        }).WithName(nameof(GetShipMethods)).WithTags(nameof(ShipMethod));
    }
}


public record ShipMethodsGetAllResponse(IEnumerable<ShipMethodDto> ShipMethods);