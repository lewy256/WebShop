using ProductApi.Shared.ReviewDtos;

namespace ProductApi.Shared.LinkModels.Reviews;

public record LinkReviewParameters(ReviewParameters ReviewParameters, HttpContext Context);
