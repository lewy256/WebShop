namespace OrderApi.Models;

public class SpecOrderStatus {
    public int SpecOrderStatusId { get; set; }
    public int OrderId { get; set; }
    public int StatusId { get; set; }
    public DateTime StatusDate { get; set; }

    public virtual Order Order { get; set; }
    public virtual Status Status { get; set; }
}