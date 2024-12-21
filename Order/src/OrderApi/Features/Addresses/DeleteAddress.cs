using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using System.Security.Claims;

namespace OrderApi.Features.Addresses;

public static class DeleteAddress {
    public sealed record Command(int Id) : IRequest<AddressDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, AddressDeleteResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var rows = await _context.Address
                .Where(a => a.AddressId == request.Id && a.CustomerId
                .Equals(userId))
                .ExecuteDeleteAsync(cancellationToken);

            if(rows == 0) {
                return new NotFoundResponse(request.Id.ToString(), nameof(Address));
            }

            return new Success();
        }
    }
}

public class DeleteAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/addresses/{id}",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteAddress.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.Problem(notfound));

        }).WithName(nameof(DeleteAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}