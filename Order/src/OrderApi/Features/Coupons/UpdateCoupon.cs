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

namespace OrderApi.Features.Coupons;

public static class UpdateCoupon {
    public class Command : IRequest<CouponUpdateResponse> {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }

    public class Validator : AbstractValidator<Command> {
        public Validator() {
            RuleFor(x => x.Description)
              .NotEmpty();
        }
    }

    internal sealed class UpdateOrderHandler : IRequestHandler<Command, CouponUpdateResponse> {
        private readonly OrderContext _context;
        private readonly IValidator<Command> _validator;

        public UpdateOrderHandler(OrderContext orderContext, IValidator<Command> validator) {
            _context = orderContext;
            _validator = validator;
        }

        public async ValueTask<CouponUpdateResponse> Handle(Command request, CancellationToken cancellationToken) {
            var validationResult = await _validator.ValidateAsync(request);

            if(!validationResult.IsValid) {

                var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();

                return new ValidationResponse(vaildationFailed);
            }

            var coupon = await _context.Coupon.SingleOrDefaultAsync(p => p.CouponId == request.Id);

            if(coupon is null) {
                return new NotFoundResponse(request.Id, nameof(Coupon));
            }

            request.Adapt(coupon);

            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}

public class UpdateStatusEndpoint : ICarterModule {
    public void AddRoutes(IEndpointRouteBuilder app) {
        app.MapPut("api/coupons/{id}",
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        async (int id, [FromBody] CouponRequest request, ISender sender) => {
            var command = request.Adapt<UpdateCoupon.Command>();

            command.Id = id;

            var results = await sender.Send(command);

            return results.Match(
                _ => Results.NoContent(),
                validationFailed => Results.UnprocessableEntity(validationFailed),
                notfound => Results.NotFound(notfound));

        }).WithName(nameof(UpdateCoupon)).WithTags(nameof(Coupon));
    }
}

[GenerateOneOf]
public partial class CouponUpdateResponse : OneOfBase<Success, ValidationResponse, NotFoundResponse> {
}
