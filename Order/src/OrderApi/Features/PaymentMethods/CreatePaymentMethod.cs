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

namespace OrderApi.Features.PaymentMethods;

public static class CreatePaymentMethod {
    public class Command : IRequest<PaymentMethodCreateResponse> {
        public string Name { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Name)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, PaymentMethodCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext context, IValidator<Command> validator) {
            _context = context;
            _validator = validator;
        }

        public async ValueTask<PaymentMethodCreateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var paymentMethod = request.Adapt<PaymentMethod>();

            await _context.PaymentMethod.AddAsync(paymentMethod);
            await _context.SaveChangesAsync(cancellationToken);

            var paymentMethodDto = paymentMethod.Adapt<PaymentMethodDto>();

            return paymentMethodDto;

        }
    }
}

public class CreateStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/payment-methods",
                  [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        async ([FromBody] PaymentMethodRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<CreatePaymentMethod.Command>();

            var results = await sender.Send(command);

            return results.Match(
                paymentMethod => Results.Created($"{nameof(PaymentMethod)}/{paymentMethod.PaymentMethodId}", paymentMethod),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreatePaymentMethod)).WithTags(nameof(PaymentMethod));
    }
}

[GenerateOneOf]
public partial class PaymentMethodCreateResponse : OneOfBase<PaymentMethodDto, ValidationResponse> {
}