using Microsoft.Net.Http.Headers;
using ProductApi.Interfaces;
using ProductApi.Model.LinkModels;
using ProductApi.Model.LinkModels.Reviews;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Utility;

public class ReviewLinks : IReviewLinks {
    private readonly LinkGenerator _linkGenerator;

    public ReviewLinks(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    public ReviewLinkResponse TryGenerateLinks(IEnumerable<ReviewDto> reviews, Guid productId, HttpContext httpContext) {

        if(ShouldGenerateLinks(httpContext)) {
            return ReturnLinkdedReviews(reviews, productId, httpContext);
        }

        return new ReviewLinkResponse() { HasLinks = false, Reviews = reviews };
    }

    private bool ShouldGenerateLinks(HttpContext httpContext) {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private ReviewLinkResponse ReturnLinkdedReviews(IEnumerable<ReviewDto> reviews, Guid productId, HttpContext httpContext) {
        var reviewList = reviews.ToList();
        var linkedReviews = new List<LinkedReviews>();

        foreach(var item in reviewList) {
            var reviewLinks = CreateLinksForReview(httpContext, productId, item.Id);

            linkedReviews.Add(new LinkedReviews() {
                Id = item.Id,
                Description = item.Description,
                Rating = item.Rating,
                Links = reviewLinks
            });
        }

        var entity = new LinkedReviewEntity() {
            Value = linkedReviews,
            Links = CreateLinksForReviews(httpContext)
        };

        return new ReviewLinkResponse { HasLinks = true, LinkedEntity = entity };
    }

    private List<Link> CreateLinksForReview(HttpContext httpContext, Guid productId, Guid reviewId) {
        var links = new List<Link>{
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetReviewForProduct", values: new { productId,reviewId}),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteReviewForProduct", values: new { productId,reviewId }),
                "delete_review",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateReviewForProduct", values: new { productId,reviewId}),
                "update_review",
                "PUT")
            };
        return links;
    }

    private List<Link> CreateLinksForReviews(HttpContext httpContext) {
        var links = new List<Link>{
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetReviewsForProduct", values: new {}),
                "self",
                "GET")
            };
        return links;
    }
}
