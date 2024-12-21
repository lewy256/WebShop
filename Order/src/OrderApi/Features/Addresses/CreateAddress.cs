using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared;
using OrderApi.Shared.AddressDtos;
using System.Security.Claims;
using static OrderApi.Features.Addresses.CreateAddress;

namespace OrderApi.Features.Addresses;

public static class CreateAddress {
    public class AddressRequest : IRequest<AddressCreateResponse> {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string AddressLine1 { get; init; }
        public string AddressLine2 { get; init; }
        public string PostalCode { get; init; }
        public string PhoneNumber { get; init; }
        public string Country { get; init; }
        public string City { get; init; }
    }

    public class Validator : AbstractValidator<AddressRequest> {
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

    internal sealed class Handler : IRequestHandler<AddressRequest, AddressCreateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<AddressRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(OrderContext context, IValidator<AddressRequest> validator, IHttpContextAccessor httpContextAccessor) {
            _context = context;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<AddressCreateResponse> Handle(AddressRequest request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {
                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var address = request.Adapt<Address>();

            var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
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
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async ([FromBody] AddressRequest address, ISender sender) => {
            var results = await sender.Send(address);

            return results.Match(
                address => Results.Created($"{nameof(Address)}/{address.Id}", address),
                validationFailed => Results.Problem(validationFailed));

        }).WithName(nameof(CreateAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressCreateResponse : OneOfBase<AddressDto, ValidationResponse> {
}