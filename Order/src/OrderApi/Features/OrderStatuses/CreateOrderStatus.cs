using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.OrderStatus;

public static class CreateOrderStatus {
    public class Command : IRequest<OrderStatusCreateResponse> {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.OrderId)
             .NotEmpty();
            RuleFor(x => x.StatusId)
             .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, OrderStatusCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext context, IValidator<Command> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<OrderStatusCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var order = _context.Order.AsNoTracking().SingleOrDefaultAsync(x => x.OrderId.Equals(request.OrderId));

            if(order is null) {
                return new NotFoundResponse(request.OrderId, nameof(Order));
            }

            var status = _context.Status.AsNoTracking().SingleOrDefaultAsync(x => x.StatusId == request.StatusId);

            if(order is null) {
                return new NotFoundResponse(request.StatusId, nameof(Status));
            }

            var specOrderStatus = new SpecOrderStatus() {
                OrderId = request.OrderId,
                StatusId = request.StatusId,
                StatusDate = DateTime.UtcNow
            };

            await _context.SpecOrderStatus.AddAsync(specOrderStatus);
            await _context.SaveChangesAsync(cancellationToken);

            var specOrderStatusDto = specOrderStatus.Adapt<SpecOrderStatusDto>();

            return specOrderStatusDto;
        }
    }
}

public class CreateSpecOrderStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/order-statuses",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(SpecOrderStatus), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] OrderStatusRequest request, ISender sender) => {
            var command = request.Adapt<CreateOrderStatus.Command>();

            var results = await sender.Send(command);

            return results.Match(
                specOrderStatus => Results.Created($"{nameof(SpecOrderStatus)}/{specOrderStatus.SpecOrderStatusId}", specOrderStatus),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notFound => Results.NotFound(notFound));

        }).WithName(nameof(CreateOrderStatus)).WithTags(nameof(SpecOrderStatus));
    }
}

[GenerateOneOf]
public partial class OrderStatusCreateResponse : OneOfBase<SpecOrderStatusDto, ValidationResponse, NotFoundResponse> {
}