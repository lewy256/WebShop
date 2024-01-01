using OneOf;
using OneOf.Types;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Model.Responses;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.Interfaces;

public interface IReviewService {
    Task<ReviewtGetAllResponse> GetReviewsAsync(Guid productId, LinkReviewParameters linkParameters);
    Task<ReviewGetResponse> GetReviewByIdAsync(Guid productId, Guid reviewId);
    Task<ReviewCreateResponse> CreateReviewAsync(Guid productId, CreateReviewDto reviewDto);
    Task<ReviewUpdateResponse> UpdateReviewAsync(Guid productId, Guid reviewId, UpdateReviewDto reviewDto);
    Task<ReviewDeleteResponse> DeleteReviewAsync(Guid productId, Guid reviewId);
}


[GenerateOneOf]
public partial class ReviewUpdateResponse : OneOfBase<Success, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class ReviewCreateResponse : OneOfBase<ReviewDto, NotFoundResponse, ValidationFailed> {
}

[GenerateOneOf]
public partial class ReviewDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ReviewGetResponse : OneOfBase<ReviewDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ReviewtGetAllResponse : OneOfBase<(ReviewLinkResponse linkResponse, MetaData metaData), NotFoundResponse, ValidationFailed> {
}
