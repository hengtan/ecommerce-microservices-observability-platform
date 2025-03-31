using EcommerceModular.Application.DTOs;
using EcommerceModular.Domain.Events;
using Newtonsoft.Json;

namespace Orders.Consumer.Dtos;

public class OrderDto
{
    [JsonProperty("OrderId")]
    public Guid OrderId { get; set; }

    [JsonProperty("CustomerId")]
    public Guid CustomerId { get; set; }

    [JsonProperty("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("ShippingAddress")]
    public ShippingAddressDto ShippingAddress { get; set; }

    [JsonProperty("Items")]
    public List<OrderItemDto> Items { get; set; }
}