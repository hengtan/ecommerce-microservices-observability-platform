using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Orders.Projections;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using IOrderReadProjection = EcommerceModular.Application.Interfaces.Persistence.IOrderReadProjection;

namespace EcommerceModular.Infrastructure.Persistence;

public class OrderReadProjection : IOrderReadProjection
{
    private readonly IMongoCollection<OrderReadModel> _collection;

    public OrderReadProjection(IOptions<MongoSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var db = client.GetDatabase(options.Value.DatabaseName);
        _collection = db.GetCollection<OrderReadModel>("orders_read");
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken = default)
    {
        var readModel = new OrderReadModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            ShippingCity = order.ShippingAddress.City,
            Items = order.Items.Select(i => new OrderItemReadModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity
            }).ToList()
        };

        await _collection.InsertOneAsync(readModel, cancellationToken: cancellationToken);
    }
}