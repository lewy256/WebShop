using Carter;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;

namespace OrderApi.Features.Coupons;

public static class DeleteCoupon {
    public sealed record Command(int Id) : IRequest<StatusDeleteResponse>;
    internal sealed class Handler : IRequestHandler<Command, StatusDeleteResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<StatusDeleteResponse> Handle(Command request, CancellationToken cancellationToken) {
            var rows = await _context.Coupon.Where(a => a.CouponId == request.Id).ExecuteDeleteAsync();

            if(rows == 0) {
                return new NotFoundResponse(request.Id, nameof(Address));
            }

            return new Success();
        }

    }
}

public class DeleteStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapDelete("api/coupons/{id}",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new DeleteCoupon.Command(id);

            var results = await sender.Send(query);

            return results.Match(
                _ => Results.NoContent(),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(DeleteCoupon)).WithTags(nameof(Coupon));
    }
}

[GenerateOneOf]
public partial class StatusDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}