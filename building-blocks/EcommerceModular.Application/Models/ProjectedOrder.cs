namespace EcommerceModular.Application.Models;

public class ProjectedOrder
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = null!;
    public List<ProjectedOrderItem> Items { get; set; } = [];
}