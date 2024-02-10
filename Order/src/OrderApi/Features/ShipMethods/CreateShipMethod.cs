using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.ShipMethods;

public static class CreateShipMethod {
    public class Command : IRequest<ShipMethodCreateResponse> {
        public string Description { get; set; }
        public DateTime DeliveryTime { get; set; }
        public decimal Price { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Description)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, ShipMethodCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext context, IValidator<Command> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<ShipMethodCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var shipMethod = request.Adapt<ShipMethod>();

            await _context.ShipMethod.AddAsync(shipMethod);
            await _context.SaveChangesAsync(cancellationToken);

            var shipMetodDto = shipMethod.Adapt<ShipMethodDto>();

            return shipMetodDto;
        }
    }
}

public class CreateShipMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/ship-methods",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ShipMethodDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] ShipMethodRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<CreateShipMethod.Command>();

            var results = await sender.Send(command);

            return results.Match(
                shipMethod => Results.Created($"{nameof(ShipMethod)}/{shipMethod.ShipMethodId}", shipMethod),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreateShipMethod)).WithTags(nameof(ShipMethod));
    }
}

[GenerateOneOf]
public partial class ShipMethodCreateResponse : OneOfBase<ShipMethodDto, ValidationResponse> {
}