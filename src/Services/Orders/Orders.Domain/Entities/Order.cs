namespace Orders.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }

    public decimal Total => Items.Sum(item => item.Total);

    public Order(string customerId, Address shippingAddress, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        Items = items;
        Status = OrderStatus.Pending;
    }

    public void MarkAsPaid()
    {
        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        Status = OrderStatus.Canceled;
    }
}
