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
using System.Security.Claims;

namespace OrderApi.Features.Addresses;

public static class CreateAddress {
    public class Command : IRequest<AddressCreateResponse> {
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

    internal sealed class Handler : IRequestHandler<Command, AddressCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IValidator<Command> validator, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressCreateResponse> Handle(Command request, CancellationToken cancellationToken) {


            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {
                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var address = request.Adapt<Address>();

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            address.CustomerId = new Guid(userId);

            await _context.Address.AddAsync(address);

            await _context.SaveChangesAsync(cancellationToken);

            var addressDto = address.Adapt<AddressDto>();

            return addressDto;
        }
    }
}

public class CreateAddressEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPost("api/addresses",
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] AddressRequest request, ISender sender) => {
            var command = request.Adapt<CreateAddress.Command>();

            var results = await sender.Send(command);

            return results.Match(
                address => Results.Created($"{nameof(Address)}/{address.AddressId}", address),
                validationFailed => Results.UnprocessableEntity(validationFailed));

        }).WithName(nameof(CreateAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressCreateResponse : OneOfBase<AddressDto, ValidationResponse> {
}