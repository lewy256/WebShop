using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.Addresses;

public static class DeleteAddress {
    public sealed record Command(int Id) : IRequest<AddressDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, AddressDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<AddressDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            var rows = await _context.Address.Where(a => a.AddressId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(Address));
            }

            return new Success();
        }
    }
}

public class DeleteAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/addresses/{id}",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteAddress.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}