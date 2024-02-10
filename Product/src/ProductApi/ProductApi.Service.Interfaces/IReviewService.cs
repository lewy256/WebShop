using OneOf;
using OneOf.Types;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.Responses;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.Interfaces;

public interface IReviewService {
    Task<ReviewtGetAllResponse> GetReviewsAsync(Guid productId, ReviewParameters reviewParameters);
    Task<ReviewGetResponse> GetReviewByIdAsync(Guid productId, Guid reviewId);
    Task<ReviewCreateResponse> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto);
    Task<ReviewUpdateResponse> UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto);
    Task<ReviewDeleteResponse> DeleteReviewAsync(Guid productId, Guid reviewId);
}


[GenerateOneOf]
public partial class ReviewUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ReviewCreateResponse : OneOfBase<ReviewDto, NotFoundResponse, ValidationResponse> {
}

[GenerateOneOf]
public partial class ReviewDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ReviewGetResponse : OneOfBase<ReviewDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ReviewtGetAllResponse : OneOfBase<(IEnumerable<ReviewDto> reviews, MetaData metaData), NotFoundResponse, ValidationResponse> {
}