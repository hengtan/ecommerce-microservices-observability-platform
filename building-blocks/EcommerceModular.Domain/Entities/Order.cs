using Newtonsoft.Json;

namespace EcommerceModular.Domain.Entities;

public class Order
{
    [JsonProperty("OrderId")]
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public Address ShippingAddress { get; private set; }
    public OrderStatus Status { get; private set; }

    public decimal Total => Items.Sum(item => item.Total);

    // 👇 Construtor vazio obrigatório para o EF Core
    private Order() { }

    // Usado ao criar a order no fluxo de escrita
    public Order(string customerId, Address shippingAddress, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        Items = items;
        Status = OrderStatus.Pending;
    }

    // Novo construtor usado no fluxo de leitura (projeção)
    public Order(Guid id, string customerId, DateTime createdAt, Address shippingAddress, List<OrderItem> items, OrderStatus status)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        ShippingAddress = shippingAddress;
        Items = items;
        Status = status;
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
