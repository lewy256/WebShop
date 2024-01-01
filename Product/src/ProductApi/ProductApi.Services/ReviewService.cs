using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Model.Responses;
using ProductApi.Service.Extensions;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service;

public class ReviewService : IReviewService {
    private readonly ProductContext _productContext;
    private readonly IReviewLinks _reviewLinks;
    private readonly IValidator<CreateReviewDto> _createValidator;
    private readonly IValidator<UpdateReviewDto> _updateValidator;
    private readonly IValidator<ReviewParameters> _parametersValidator;

    public ReviewService(ProductContext productContext, IReviewLinks reviewLinks,
        IValidator<CreateReviewDto> createValidator,
        IValidator<UpdateReviewDto> updateValidator,
        IValidator<ReviewParameters> parametersValidator) {
        _productContext = productContext;
        _reviewLinks = reviewLinks;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _parametersValidator = parametersValidator;
    }


    public async Task<ReviewtGetAllResponse> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters) {
        var validationResult = await _parametersValidator.ValidateAsync(linkParameters.ReviewParameters);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var reviews = await _productContext.Review
            .AsNoTracking()
            .Where(p => p.ProductId.Equals(productId))
            .SortReviews(linkParameters.ReviewParameters.OrderBy)
            .Skip((linkParameters.ReviewParameters.PageNumber - 1) * linkParameters.ReviewParameters.PageSize)
            .Take(linkParameters.ReviewParameters.PageSize)
            .ToListAsync();

        var productsDto = reviews.Adapt<IEnumerable<ReviewDto>>();

        var count = await _productContext.Review
            .Where(p => p.ProductId.Equals(productId))
            .CountAsync();

        var links = _reviewLinks.TryGenerateLinks(productsDto, productId, linkParameters.Context);

        return (linkResponse: links, metaData: new MetaData() {
            CurrentPage = linkParameters.ReviewParameters.PageNumber,
            PageSize = linkParameters.ReviewParameters.PageSize,
            TotalCount = count
        });
    }

    public async Task<ReviewGetResponse> GetReviewByIdAsync(Guid productId, Guid reviewId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var review = await _productContext.Review.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            return new NotFoundResponse(reviewId, nameof(review));
        }

        var reviewDto = review.Adapt<ReviewDto>();

        return reviewDto;
    }

    public async Task<ReviewCreateResponse> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto) {
        var validationResult = await _createValidator.ValidateAsync(reviewDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }

        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var review = reviewDto.Adapt<Review>();

        review.Id = Guid.NewGuid();
        review.ProductId = product.Id;
        review.ReviewDate = DateTime.UtcNow;
        review.Discriminator = nameof(Review);

        await _productContext.AddAsync(review);
        await _productContext.SaveChangesAsync();

        var reviewToReturn = review.Adapt<ReviewDto>();

        return reviewToReturn;
    }

    public async Task<ReviewUpdateResponse> UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto) {
        var validationResult = await _updateValidator.ValidateAsync(reviewDto);

        if(!validationResult.IsValid) {
            var vaildationFailed = validationResult.Errors.Adapt<IEnumerable<ValidationError>>();
            return new ValidationFailed(vaildationFailed);
        }


        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var review = await _productContext.Review.SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            return new NotFoundResponse(reviewId, nameof(review));
        }

        reviewDto.Adapt(review);

        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public async Task<ReviewDeleteResponse> DeleteReviewAsync(Guid productId, Guid reviewId) {
        var product = await _productContext.Product.AsNoTracking().SingleOrDefaultAsync(c => c.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(product));
        }

        var review = await _productContext.Review.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(reviewId));

        if(review is null) {
            return new NotFoundResponse(reviewId, nameof(review));
        }

        _productContext.Review.Remove(review);

        await _productContext.SaveChangesAsync();

        return new Success();
    }
}