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

namespace OrderApi.Features.Addresses;

public static class UpdateAddress {
    public class Command : IRequest<AddressUpdateResponse> {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.FirstName)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, AddressUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext orderContext, IValidator<Command> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<AddressUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var address = await _context.Address.SingleOrDefaultAsync(p => p.AddressId == request.Id);

            if(address is null) {
                return new NotFoundResponse(request.Id, nameof(Address));
            }

            request.Adapt(address);

            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}

public class UpdateAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPut("api/addresses/{id}",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] AddressRequest request, ISender sender) => {
            var command = request.Adapt<UpdateAddress.Command>();

            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(UpdateAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
