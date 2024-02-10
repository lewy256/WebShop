namespace OrderApi.Shared;

public class OrderParameters {
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}