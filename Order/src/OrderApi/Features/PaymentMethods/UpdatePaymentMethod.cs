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

namespace OrderApi.Features.PaymentMethods;

public static class UpdatePaymentMethod {
    public class Command : IRequest<PaymentMethodUpdateResponse> {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Name)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, PaymentMethodUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext orderContext, IValidator<Command> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<PaymentMethodUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var paymentMethod = await _context.PaymentMethod.SingleOrDefaultAsync(p => p.PaymentMethodId == request.Id);

            if(paymentMethod is null) {
                return new NotFoundResponse(request.Id, nameof(PaymentMethod));
            }

            request.Adapt(paymentMethod);

            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}

public class UpdatePaymentMethodEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPut("api/payment-methods/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] PaymentMethodRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<UpdatePaymentMethod.Command>();

            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(UpdatePaymentMethod)).WithTags(nameof(PaymentMethod));
    }
}

[GenerateOneOf]
public partial class PaymentMethodUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
