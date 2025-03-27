using System.Text.Json;
using EcommerceModular.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace EcommerceModular.Application.Interfaces.ReadModels;

public class OrderReadService(IDistributedCache cache, IMongoClient mongoClient) : IOrderReadService
{
    private readonly IMongoCollection<Order> _orderCollection = mongoClient
        .GetDatabase("orders_read")
        .GetCollection<Order>("orders");

    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";

        // Try Redis
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<Order>(cached);
        }

        // Fallback to MongoDB
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
}