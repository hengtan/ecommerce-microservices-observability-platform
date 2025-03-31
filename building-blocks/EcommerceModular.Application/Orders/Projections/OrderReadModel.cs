using EcommerceModular.Application.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceModular.Application.Orders.Projections;

public class OrderReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }

    // NOVO
    public AddressDto ShippingAddress { get; set; }
    public string ShippingCity => ShippingAddress?.City;

    public List<OrderItemReadModel> Items { get; set; } = new();
}

public class OrderItemReadModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}