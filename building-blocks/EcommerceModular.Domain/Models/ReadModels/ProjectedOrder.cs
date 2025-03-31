namespace EcommerceModular.Domain.Models.ReadModels;

public class ProjectedOrder
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = null!;
    public string ShippingCity { get; set; } = null!; // ✅ Adicionado aqui!
    public List<ProjectedOrderItem> Items { get; set; } = [];
}