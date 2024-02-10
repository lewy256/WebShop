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

namespace OrderApi.Features.Statuses;

public static class UpdateStatus {
    public class Command : IRequest<StatusUpdateResponse> {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Description)
              .NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, StatusUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(OrderContext orderContext, IValidator<Command> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<StatusUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var status = await _context.Status.SingleOrDefaultAsync(p => p.StatusId.Equals(request.Id));

            if(status is null) {
                return new NotFoundResponse(request.Id, nameof(status));
            }

            request.Adapt(status);

            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}

public class UpdateStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPut("api/statuses/{id}",
         [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] StatusRequest? request, ISender sender) => {
            if(request is null) {
                return Results.BadRequest(new BadRequestResponse());
            }

            var command = request.Adapt<UpdateStatus.Command>();
            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(UpdateStatus)).WithTags(nameof(Status));
    }
}

[GenerateOneOf]
public partial class StatusUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
