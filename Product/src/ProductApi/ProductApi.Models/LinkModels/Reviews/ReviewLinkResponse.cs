using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Model.LinkModels.Reviews;
public class ReviewLinkResponse {
    public bool HasLinks { get; set; }
    public IEnumerable<ReviewDto> Reviews { get; set; }
    public LinkedReviewEntity LinkedEntity { get; set; }
}
