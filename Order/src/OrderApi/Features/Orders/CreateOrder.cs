using Carter;
using FluentValidation;
using Mapster;
using MassTransit;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;
using System.Security.Claims;

namespace OrderApi.Features.Orders;

public static class CreateOrder {
    public class Command : IRequest<OrderCreateResponse> {
        public DateTime OrderDate { get; set; }
        public int PaymentMethodId { get; set; }
        public int AddressId { get; set; }
        public int ShipMethodId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public int? CouponId { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {

        }
    }

    internal sealed class Handler : IRequestHandler<Command, OrderCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(OrderContext context, IValidator<Command> validator, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint) {
            _context = context;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _publishEndpoint = publishEndpoint;
        }

        public async ValueTask<OrderCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var order = request.Adapt<Order>();

            var userId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            order.CustomerId = userId;




            return new Success();
        }
    }
}

public class CreateOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/orders",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] OrderRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<CreateOrder.Command>();

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.Accepted(),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreateOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderCreateResponse : OneOfBase<Success, ValidationResponse> {
}
