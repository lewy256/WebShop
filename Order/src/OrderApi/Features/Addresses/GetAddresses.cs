using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Shared.AddressDtos;
using System.Security.Claims;

namespace OrderApi.Features.Addresses;

public static class GetAddresses {
    public sealed record Query : IRequest<AddressesGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, AddressesGetAllResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressesGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var addressDtos = await _context.Address
                .AsNoTracking()
                .Where(x => x.CustomerId.Equals(userId))
                .ProjectToType<AddressDto>()
                .ToListAsync();

            return new AddressesGetAllResponse(addressDtos);
        }
    }
}

public class GetAddressesEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/addresses",
        [Authorize(Policy = "RequireMultipleRoles")]
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