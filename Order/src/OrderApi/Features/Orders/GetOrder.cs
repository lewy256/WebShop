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
using OrderApi.Shared;
using OrderApi.Shared.AddressDtos;
using OrderApi.Shared.OrderDtos;
using System.Security.Claims;

namespace OrderApi.Features.Orders;

public static class GetOrder {
    public sealed record Query(int id) : IRequest<OrderGetResponse>;
    internal sealed class Handler : IRequestHandler<Query, OrderGetResponse> {
        private readonly OrderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<OrderGetResponse> Handle(Query request, CancellationToken cancellationToken) {
            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var order = await _context.Order
                .AsSplitQuery()
                .AsNoTracking()
                .Include(o => o.PaymentMethod)
                .Include(o => o.Address)
                .Include(o => o.ShipMethod)
                .Include(o => o.Coupon)
                .Include(o => o.OrderItem)
                .SingleOrDefaultAsync(o => o.OrderId == request.id && o.CustomerId.Equals(userId));

            if(order is null) {
                return new NotFoundResponse(request.id.ToString(), nameof(Order));
            }

            var orderSummaryDto = new OrderSummaryDto() {
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                PaymentMethod = order.PaymentMethod.Adapt<PaymentMethodDto>(),
                Address = order.Address.Adapt<AddressDto>(),
                ShipMethod = order.ShipMethod.Adapt<ShipMethodDto>(),
                DiscountAmount = order.DiscountAmount,
                TotalPrice = order.TotalAmount,
                Notes = order.Notes,
                Coupon = order.Coupon.Adapt<CouponDto>(),
                OrderItems = order.OrderItem.Adapt<List<OrderItemDto>>(),
                Statuses = order.SpecOrderStatus.Where(s => s.OrderId == order.OrderId).Select(d => d.Status.Description).Adapt<List<StatusDto>>(),

            };

            return orderSummaryDto;

        }
    }
}

public class GetOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapGet("api/orders/{id}",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, ISender sender) => {
            var query = new GetOrder.Query(id);

            var results = await sender.Send(query);

            return results.Match(
                order => Results.Ok(order),
                notfound => Results.Problem(notfound));

        }).WithName(nameof(GetOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderGetResponse : OneOfBase<OrderSummaryDto, NotFoundResponse> {
}