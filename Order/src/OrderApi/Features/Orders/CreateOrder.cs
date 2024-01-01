using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using OneOf;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared.OrderDtos;
using static OrderApi.Features.Orders.CreateOrder;

namespace OrderApi.Features.Orders;

public static class CreateOrder {
    public sealed record CreateOrderCommand(CreateOrderDto Order) : IRequest<OrderCreateResponse>;

    public class CreateOrderValidator : AbstractValidator<CreateOrderDto> {
        public CreateOrderValidator() {
            RuleFor(x => x.Notes)
                .NotEmpty();
        }
    }

    internal sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<CreateOrderDto> _validator;

        public CreateOrderHandler(OrderContext context, IValidator<CreateOrderDto> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<OrderCreateResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken) {
            if(request.Order is null) {
                return new BadRequestResponse();
            }

            var validationResult = await _validator.ValidateAsync(request.Order);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationFailed(vaildationFailed);
            }

            var order = request.Order.Adapt<Order>();

            await _context.AddAsync(order);
            await _context.SaveChangesAsync(cancellationToken);

            var orderDto = order.Adapt<OrderDto>();

            return orderDto;

        }
    }
}

public class CreateOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/orders", async (CreateOrderDto order, ISender sender) => {
            var command = new CreateOrderCommand(order);

            var results = await sender.Send(command);

            return results.Match(
                order => Results.CreatedAtRoute("GetOrderById", new { orderId = order.OrderId }, order),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                _ => Results.BadRequest());
        });
    }
}

[GenerateOneOf]
public partial class OrderCreateResponse : OneOfBase<OrderDto, ValidationFailed, BadRequestResponse> {
}