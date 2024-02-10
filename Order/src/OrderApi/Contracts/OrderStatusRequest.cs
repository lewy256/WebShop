namespace OrderApi.Contracts;

public class OrderStatusRequest {
    public int OrderId { get; set; }
    public int StatusId { get; set; }
}
