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

namespace OrderApi.Features.Coupons;

public static class GetCoupon {
    public sealed record Query(int Id) : IRequest<CouponGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, CouponGetResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<CouponGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var couponDto = await _context.Coupon.AsNoTracking().ProjectToType<CouponDto>().SingleOrDefaultAsync(o => o.CouponId == request.Id);

            if(couponDto is null) {
                return new NotFoundResponse(request.Id, nameof(Coupon));
            }

            return couponDto;
        }
    }
}

public class GetStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/coupons/{id}",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetCoupon.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                coupon => Results.Ok(coupon),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(GetCoupon)).WithTags(nameof(Coupon));
    }
}

[GenerateOneOf]
public partial class CouponGetResponse : OneOfBase<CouponDto, NotFoundResponse> {
}