using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared;
using OrderApi.Shared.AddressDtos;
using System.Security.Claims;

namespace OrderApi.Features.Addresses;

public static class UpdateAddress {
    public class Command : IRequest<AddressUpdateResponse> {
        public int Id { get; set; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string AddressLine1 { get; init; }
        public string AddressLine2 { get; init; }
        public string PostalCode { get; init; }
        public string PhoneNumber { get; init; }
        public string Country { get; init; }
        public string City { get; init; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);
            RuleFor(x => x.AddressLine1)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.AddressLine2)
                .MaximumLength(100);
            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .MaximumLength(10);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(50);
            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(50);
        }
    }

    internal sealed class Handler : IRequestHandler<Command, AddressUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext orderContext, IValidator<Command> validator, IHttpContextAccessor httpContextAccessor) {
            _context = orderContext;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var userId = new Guid(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var address = await _context.Address
                .SingleOrDefaultAsync(x => x.AddressId == request.Id && x.CustomerId.Equals(userId));

            if(address is null) {
                return new NotFoundResponse(request.Id.ToString(), nameof(Address));
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
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] UpdateAddressDto address, ISender sender) => {
            var command = address.Adapt<UpdateAddress.Command>();

            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.Problem(validationFailed),
                notfound => Results.Problem(notfound));

        }).WithName(nameof(UpdateAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
