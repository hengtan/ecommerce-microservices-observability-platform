using System.Text.Json;
using EcommerceModular.Application.Orders.Projections;
using EcommerceModular.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace EcommerceModular.Application.Interfaces.ReadModels;

public class OrderReadService(IMongoClient mongoClient, IDistributedCache cache) : IOrderReadService
{
    private readonly IMongoCollection<OrderReadModel> _orderCollection = mongoClient
        .GetDatabase("orders_read")
        .GetCollection<OrderReadModel>("orders");

    public async Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<OrderReadModel>(cached);
        }

        var order = await _orderCollection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

        if (order is not null)
        {
            var serialized = JsonSerializer.Serialize(order);
            await cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            }, cancellationToken);
        }

        return order;
    }

    private static OrderReadModel MapToReadModel(Order order)
    {
        return new OrderReadModel
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
    }
}