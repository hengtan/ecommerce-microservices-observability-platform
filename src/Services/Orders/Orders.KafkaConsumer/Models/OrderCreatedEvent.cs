namespace Orders.KafkaConsumer.Models;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}