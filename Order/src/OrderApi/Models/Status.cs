namespace OrderApi.Models;
public class Status {
    public Status() {
        SpecOrderStatus = new HashSet<SpecOrderStatus>();
    }

    public int StatusId { get; set; }
    public string Description { get; set; }

    public virtual ICollection<SpecOrderStatus> SpecOrderStatus { get; set; }
}