using System.Text.Json;
using EcommerceModular.Application.Orders.Projections;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace EcommerceModular.Application.Interfaces.ReadModels;

public class OrderReadService(
    IDistributedCache cache,
    IMongoClient mongoClient)
    : IOrderReadService
{
    private readonly IMongoCollection<OrderReadModel> _collection =
        mongoClient
            .GetDatabase("orders_read")
            .GetCollection<OrderReadModel>("orders");

    public async Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";

        // ✅ 1. Tenta pegar do Redis
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            return JsonSerializer.Deserialize<OrderReadModel>(cached);
        }

        // ✅ 2. Fallback: consulta no MongoDB
        var order = await _collection
            .Find(o => o.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        // ✅ 3. Se achou no Mongo, salva no Redis para próximas leituras
        if (order is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            var serialized = JsonSerializer.Serialize(order);
            await cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
        }

        return order;
    }
}