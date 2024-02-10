using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Contracts;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;

namespace OrderApi.Features.ShipMethods;

public static class UpdateShipMethod {
    public class Command : IRequest<ShipMethodUpdateResponse> {
        public int Id { get; set; }
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

    internal sealed class Handler : IRequestHandler<Command, ShipMethodUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext orderContext, IValidator<Command> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<ShipMethodUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var shipMethod = await _context.ShipMethod.SingleOrDefaultAsync(p => p.ShipMethodId == request.Id);

            if(shipMethod is null) {
                return new NotFoundResponse(request.Id, nameof(ShipMethod));
            }

            request.Adapt(shipMethod);

            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}

public class UpdateShipMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPut("api/ship-methods/{id}",
         [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] ShipMethodRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<UpdateShipMethod.Command>();

            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(UpdateShipMethod)).WithTags(nameof(ShipMethod));
    }
}

[GenerateOneOf]
public partial class ShipMethodUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
