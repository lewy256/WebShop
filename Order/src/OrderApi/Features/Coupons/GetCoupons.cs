using Carter;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using OrderApi.Shared;

namespace OrderApi.Features.Coupons;

public static class GetCoupons {
    public sealed record Query : IRequest<CouponsGetAllResponse>;

    internal sealed class Handler : IRequestHandler<Query, CouponsGetAllResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<CouponsGetAllResponse> Handle(Query request, CancellationToken cancellationToken) {
            var couponDtos = await _context.Coupon.AsNoTracking().ProjectToType<CouponDto>().ToListAsync();

            return new CouponsGetAllResponse(couponDtos);
        }
    }
}

public class GetStatusesEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/coupons",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (ISender sender) => {
            var query = new GetCoupons.Query();

            var results = await sender.Send(query);

            return Results.Ok(results.Coupons);

        }).WithName(nameof(GetCoupons)).WithTags(nameof(Coupon));
    }
}

public record CouponsGetAllResponse(IEnumerable<CouponDto> Coupons);