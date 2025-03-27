namespace EcommerceModular.Domain.Events;

public class OrderCreatedEvent(Guid orderId, string customerId, DateTime createdAt)
{
    public Guid OrderId { get; set; } = orderId;
    public string CustomerId { get; set; } = customerId;
    public DateTime CreatedAt { get; set; } = createdAt;
}