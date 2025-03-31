using Newtonsoft.Json;

namespace EcommerceModular.Domain.Events;

public class OrderCreatedEvent(Guid orderId, string customerId, DateTime createdAt, ShippingAddressDto shippingAddress, List<OrderItemDto> items)
{
    public Guid OrderId { get; set; } = orderId;
    public string CustomerId { get; set; } = customerId;
    public DateTime CreatedAt { get; set; } = createdAt;
    public ShippingAddressDto ShippingAddress { get; set; } = shippingAddress;
    public List<OrderItemDto> Items { get; set; } = items;
}

public class ShippingAddressDto
{
    [JsonProperty("Street")]
    public string Street { get; set; }

    [JsonProperty("City")]
    public string City { get; set; }

    [JsonProperty("State")]
    public string State { get; set; }

    [JsonProperty("Country")]
    public string Country { get; set; }

    [JsonProperty("ZipCode")]
    public string ZipCode { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}