using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using OrderApi.Shared;

namespace OrderApi.Features.Addresses;

public static class GetAddresses {
    public sealed record Query : IRequest<AddressesGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, AddressesGetAllResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<AddressesGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var addressDtos = await _context.Address.AsNoTracking().ProjectToType<AddressDto>().ToListAsync();

            return new AddressesGetAllResponse(addressDtos);
        }
    }
}

public class GetAddressesEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/addresses",
         [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender) => {
            var query = new GetAddresses.Query();

            var results = await sender.Send(query);

            return Results.Ok(results.Addresses);

        }).WithName(nameof(GetAddresses)).WithTags(nameof(Address));
    }
}

public record AddressesGetAllResponse(IEnumerable<AddressDto> Addresses);