namespace ProductApi.Shared.Model.ReviewDtos;

public class ReviewParameters {
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int? Rating { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? OrderBy { get; set; }
}