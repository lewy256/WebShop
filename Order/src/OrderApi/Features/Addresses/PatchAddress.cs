using Carter;
using FluentValidation;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OneOf;
using OneOf.Types;
using OrderApi.Models;
using OrderApi.Responses;
using OrderApi.Shared;
using System.Text.Json;

namespace OrderApi.Features.Addresses;

public static class PatchAddress {
    public record Command(int Id, JsonElement JsonElement) : IRequest<AddressPatchResponse> {
    }

    public class Validator : AbstractValidator<AddressDto> {
        public Validator() {
            RuleFor(x => x.FirstName)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, AddressPatchResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<AddressDto> _validator;

        public Handler(OrderContext orderContext, IValidator<AddressDto> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<AddressPatchResponse> Handle(Command request, CancellationToken cancellationToken) {
            var json = request.JsonElement.GetRawText();
            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument>(json, new JsonSerializerSettings() {
                Error = (sender, error) => error.ErrorContext.Handled = true
            });

            if(patchDoc is null) {
                return new BadRequestResponse("patchDoc object sent from client is null.");
            }

            var address = await _context.Address.SingleOrDefaultAsync(p => p.AddressId == request.Id);

            if(address is null) {
                return new NotFoundResponse(request.Id, nameof(Address));
            }

            var entityToPatch = address.Adapt<AddressDto>();

            patchDoc.ApplyTo(entityToPatch);

            var validationResult = await _validator.ValidateAsync(entityToPatch);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            entityToPatch.Adapt(address);

            await _context.SaveChangesAsync();

            return new Success();
        }
    }
}

public class PatchAddressEndpoint : ICarterModule {
    public async Task SaveChangesForPatchAsync(AddressDto addressToPatch, Address addressEntity, OrderContext orderContext) {
        addressToPatch.Adapt(addressEntity);

        await orderContext.SaveChangesAsync();
    }
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPatch("api/addresses/{id}",
         [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] JsonElement jsonElement, ISender sender, OrderContext orderContext) => {
            var command = new PatchAddress.Command(id, jsonElement);

            var results = await sender.Send(command);

            return results.Match(
               _ => Results.NoContent(),
               validationFailed => Results.UnprocessableEntity(validationFailed),
               notfound => Results.NotFound(notfound),
               modelIsNull => Results.BadRequest(modelIsNull));

        }).WithName(nameof(PatchAddress)).WithTags(nameof(Address));
    }
}

[GenerateOneOf]
public partial class AddressPatchResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse, BadRequestResponse> {
}
