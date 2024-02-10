using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Service.Extensions;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.Responses;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.V2;

public class ReviewService {
    private readonly ProductContext _productContext;
    private readonly IReviewLinks _reviewLinks;
    private readonly IValidator<ReviewParameters> _parametersValidator;

    public ReviewService(ProductContext productContext,
        IReviewLinks reviewLinks,
        IValidator<ReviewParameters> parametersValidator) {
        _productContext = productContext;
        _reviewLinks = reviewLinks;
        _parametersValidator = parametersValidator;
    }

    public async Task<ReviewtGetAllResponse> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.ReviewParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationResponse(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var query = _productContext.Review
        .AsNoTracking()
        .Where(p => p.ProductId.Equals(productId));

        var reviewsDto = await query
            .SortReviews(linkParameters.ReviewParameters.OrderBy)
            .Skip((linkParameters.ReviewParameters.PageNumber - 1) * linkParameters.ReviewParameters.PageSize)
            .Take(linkParameters.ReviewParameters.PageSize)
            .ProjectToType<ReviewDto>()
            .ToListAsync();

        var count = await query.CountAsync();

        var links = _reviewLinks.TryGenerateLinks(reviewsDto, productId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ReviewParameters.PageNumber,
            PageSize = linkParameters.ReviewParameters.PageSize,
            TotalCount = count
        });
    }
}

[GenerateOneOf]
public partial class ReviewtGetAllResponse : OneOfBase<(ReviewLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationResponse> {
}