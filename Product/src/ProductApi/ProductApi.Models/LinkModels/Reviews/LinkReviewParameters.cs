using Microsoft.AspNetCore.Http;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Model.LinkModels.Reviews;

public record LinkReviewParameters(ReviewParameters ReviewParameters, HttpContext Context);
