using System.Text.Json;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Domain.Models.ReadModels;
using EcommerceModular.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EcommerceModular.Infrastructure.Projections;

public class OrderReadProjection : IOrderReadProjection
{
    private readonly IMongoCollection<ProjectedOrder> _collection;

    public OrderReadProjection(IOptions<MongoSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _collection = database.GetCollection<ProjectedOrder>("orders_read");
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken)
    {
        var projected = new ProjectedOrder
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new ProjectedOrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _collection.ReplaceOneAsync(
            filter: Builders<ProjectedOrder>.Filter.Eq(o => o.Id, order.Id),
            replacement: projected,
            options: new ReplaceOptions { IsUpsert = true },
            cancellationToken: cancellationToken
        );
    }

    public async Task<ProjectedOrder?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var filter = Builders<ProjectedOrder>.Filter.Eq(o => o.Id, orderId);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}