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
using OrderApi.Entities;
using OrderApi.Infrastructure;
using OrderApi.Responses;
using OrderApi.Shared;
using OrderApi.Shared.OrderDtos;
using System.Text.Json;

namespace OrderApi.Features.Orders;

public static class PatchOrder {
    public record Command(int Id, JsonElement JsonElement) : IRequest<OrderPatchResponse> {
    }

    public class Validator : AbstractValidator<PatchOrderDto> {
        public Validator() {
            RuleFor(x => x.Notes)
                .NotEmpty()
                .MaximumLength(200);

        }
    }

    internal sealed class Handler : IRequestHandler<Command, OrderPatchResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<PatchOrderDto> _validator;

        public Handler(OrderContext orderContext, IValidator<PatchOrderDto> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<OrderPatchResponse> Handle(Command request, CancellationToken cancellationToken) {
            var json = request.JsonElement.GetRawText();
            var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument>(json, new JsonSerializerSettings() {
                Error = (sender, error) => error.ErrorContext.Handled = true
            });

            if(patchDoc is null) {
                return new BadRequestResponse();
            }

            var order = await _context.Order.SingleOrDefaultAsync(p => p.OrderId == request.Id);

            if(order is null) {
                return new NotFoundResponse(request.Id.ToString(), nameof(Order));
            }

            var entityToPatch = order.Adapt<PatchOrderDto>();

            patchDoc.ApplyTo(entityToPatch);

            var validationResult = await _validator.ValidateAsync(entityToPatch);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            entityToPatch.Adapt(order);

            await _context.SaveChangesAsync();

            return new Success();
        }
    }
}

public class PatchOrderEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPatch("api/orders/{id}",
        [Authorize(Policy = "RequireMultipleRoles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] JsonElement jsonElement, ISender sender, OrderContext orderContext) => {
            var command = new PatchOrder.Command(id, jsonElement);

            var results = await sender.Send(command);

            return results.Match(
               _ => Results.NoContent(),
               validationFailed => Results.Problem(validationFailed),
               notfound => Results.Problem(notfound),
               modelIsNull => Results.Problem(modelIsNull));

        }).WithName(nameof(PatchOrder)).WithTags(nameof(Order));
    }
}

[GenerateOneOf]
public partial class OrderPatchResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse, BadRequestResponse> {
}
