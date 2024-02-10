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

namespace OrderApi.Features.Addresses;

public static class GetAddress {
    public sealed record Query(int Id) : IRequest<AddressGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, AddressGetResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<AddressGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var addressDto = await _context.Address.AsNoTracking().ProjectToType<AddressDto>().SingleOrDefaultAsync(a => a.AddressId == request.Id);

            if(addressDto is null) {
                return new NotFoundResponse(request.Id, nameof(Address));
            }

            return addressDto;
        }
    }
}

public class GetAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/addresses/{id}",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetAddress.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressGetResponse : OneOfBase<AddressDto, NotFoundResponse> {
}