using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared.AddressDtos;
using System.Security.Claims;

namespace OrderApi.Features.Addresses;

public static class GetAddress {
    public sealed record Query(int Id) : IRequest<AddressGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, AddressGetResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var addressDto = await _context.Address
                .AsNoTracking()
                .Where(x => x.CustomerId.Equals(userId) && x.AddressId == request.Id)
                .ProjectToType<AddressDto>()
                .SingleOrDefaultAsync();


            if(addressDto is null) {
                return new NotFoundResponse(request.Id.ToString(), nameof(Address));
            }

            return addressDto;
        }
    }
}

public class GetAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/addresses/{id}",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetAddress.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.Problem(notfound));

        }).WithName(nameof(GetAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressGetResponse : OneOfBase<AddressDto, NotFoundResponse> {
}