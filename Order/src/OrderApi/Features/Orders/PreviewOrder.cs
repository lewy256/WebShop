using Carter;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OrderApi.Entities;
using OrderApi.Extensions;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared;
using OrderApi.Shared.OrderDtos;

namespace OrderApi.Features.Orders;

public class PreviewOrderRequest : IRequest<OrderPreviewResponse> {
    public string? CouponCode { get; set; }
    public List<OrderItemDto> Items { get; set; }
}


public static class PreviewOrder {
    internal sealed class Handler : IRequestHandler<PreviewOrderRequest, OrderPreviewResponse> {
        private readonly OrderContext _context;

        public Handler(OrderContext context) {
            _context = context;
        }

        public async ValueTask<OrderPreviewResponse> Handle(PreviewOrderRequest request, CancellationToken cancellationToken) {
            var orderDto = new PreviewOrderDto();
            if(request.CouponCode is null) {
                orderDto = new PreviewOrderDto() {
                    Items = request.Items,
                    TotalPrice = request.Items.Sum(item => item.Quantity * item.UnitPrice)
                };

                return orderDto;
            }

            var coupon = _context.Coupon.SingleOrDefault(c => c.Code == request.CouponCode);

            if(coupon is null) {
                return new NotFoundResponse(nameof(Coupon));
            }

            orderDto = new PreviewOrderDto() {
                CouponCode = request.CouponCode,
                Items = request.Items,
                TotalPrice = request.Items.CalculateTotalPrice(coupon.DiscountAmount)
            };

            return orderDto;
        }


    }
}

public class PreviewOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/orders/preview",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(PreviewOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        async ([FromBody] PreviewOrderRequest request, ISender sender) => {
            var results = await sender.Send(request);

            return results.Match(
                order => Results.Ok(order),
                notFound => Results.NotFound(notFound));

        }).WithName(nameof(PreviewOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderPreviewResponse : OneOfBase<PreviewOrderDto, NotFoundResponse> {
}