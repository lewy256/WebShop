using Carter;
using Contracts.Messages;
using FluentValidation;
using Mapster;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Entities;
using OrderApi.Extensions;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared;
using OrderApi.Shared.OrderDtos;
using System.Security.Claims;


namespace OrderApi.Features.Orders;

public class OrderRequest : IRequest<OrderCreateResponse> {
    public int PaymentMethodId { get; set; }
    public int AddressId { get; set; }
    public int ShipMethodId { get; set; }
    public string? Notes { get; set; }
    public string? CouponCode { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public static class CreateOrder {
    public class Validator : AbstractValidator<OrderRequest> {
        public Validator() {
            RuleFor(x => x.PaymentMethodId)
                .NotEmpty();
            RuleFor(x => x.AddressId)
                .NotEmpty();
            RuleFor(x => x.ShipMethodId)
                .NotEmpty();
            RuleFor(x => x.Notes)
                .NotEmpty()
                .MaximumLength(200);
            RuleFor(x => x.CouponCode)
                .MaximumLength(200);
            RuleFor(x => x.Items)
                .NotNull();
            RuleForEach(x => x.Items)
                .SetValidator(new ItemValidator());
        }
    }

    public class ItemValidator : AbstractValidator<OrderItemDto> {
        public ItemValidator() {
            RuleFor(x => x.ProductId)
                .NotEmpty();
            RuleFor(x => x.ProductName)
               .NotEmpty();
            RuleFor(x => x.Quantity)
               .NotEmpty();
            RuleFor(x => x.UnitPrice)
               .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<OrderRequest, OrderCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<OrderRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(OrderContext context, IValidator<OrderRequest> validator, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint) {
            _context = context;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
        }

        public async ValueTask<OrderCreateResponse> Handle(OrderRequest request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


            var paymentMethod = _context.PaymentMethod.AsNoTracking().SingleOrDefault(x => x.PaymentMethodId == request.PaymentMethodId);

            if(paymentMethod is null) {
                return new NotFoundResponse(paymentMethod.PaymentMethodId.ToString(), nameof(PaymentMethod));
            }

            var shipMethod = _context.ShipMethod.AsNoTracking().SingleOrDefault(x => x.ShipMethodId == request.ShipMethodId);

            if(paymentMethod is null) {
                return new NotFoundResponse(shipMethod.ShipMethodId.ToString(), nameof(ShipMethod));
            }

            var address = _context.Address.AsNoTracking().SingleOrDefault(x => x.AddressId == request.AddressId);

            if(address is null) {
                return new NotFoundResponse(address.AddressId.ToString(), nameof(Address));
            }

            var coupon = new Coupon();

            if(request.CouponCode is not null) {
                coupon = _context.Coupon.AsNoTracking().SingleOrDefault(x => x.Code == request.CouponCode);

                if(coupon is null) {
                    return new NotFoundResponse(coupon.CouponId.ToString(), nameof(Coupon));
                }
            }

            var order = new Order() {
                CustomerId = userId,
                OrderDate = DateTime.UtcNow,
                PaymentMethodId = paymentMethod.PaymentMethodId,
                AddressId = address.AddressId,
                ShipMethodId = shipMethod.ShipMethodId,
                DiscountAmount = coupon.DiscountAmount,
                TotalAmount = request.Items.CalculateTotalPrice(coupon.DiscountAmount),
                Notes = request.Notes,
                CouponId = coupon.CouponId,
                OrderName = Guid.NewGuid(),
            };

            await _context.Order.AddAsync(order);

            await _context.SaveChangesAsync(cancellationToken);

            var orderItems = new List<OrderItem>();

            foreach(var item in request.Items) {
                orderItems.Add(new OrderItem() {
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                });
            }

            await _context.OrderItem.AddRangeAsync(orderItems);

            await _context.SaveChangesAsync(cancellationToken);

            var messagePayload = request.Items.ToDictionary(p => p.ProductId, o => o.Quantity);

            await _publishEndpoint.Publish(new OrderCreated {
                UserId = userId.ToString(),
                Products = messagePayload
            }, cancellationToken);

            return new Success();
        }
    }
}

public class CreateOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/orders",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] OrderRequest request, ISender sender) => {
            var results = await sender.Send(request);

            return results.Match(
                _ => Results.Created(),
                notFound => Results.NotFound(notFound),
                validationFailed => Results.Problem(validationFailed));

        }).WithName(nameof(CreateOrder)).WithTags(nameof(Order));
    }
}


[GenerateOneOf]
public partial class OrderCreateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}