namespace EcommerceModular.Application.Orders.Projections;

public class OrderReadModel
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ShippingCity { get; set; }
    public List<OrderItemReadModel> Items { get; set; } = new();
}

public class OrderItemReadModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}