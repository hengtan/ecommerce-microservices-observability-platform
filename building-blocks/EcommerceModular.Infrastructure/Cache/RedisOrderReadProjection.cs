using EcommerceModular.Application.Orders.Projections;
using EcommerceModular.Application.Policies;
using EcommerceModular.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Polly;
using IOrderReadProjection = EcommerceModular.Application.Interfaces.Persistence.IOrderReadProjection;

namespace EcommerceModular.Infrastructure.Cache;

public class RedisOrderReadProjection(IDistributedCache cache, IOrderReadProjection fallback)
    : IOrderReadProjection
{
    public async Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cached))
            {
                Console.WriteLine($"🧠 [Redis] Cache hit for {id}");
                return JsonConvert.DeserializeObject<OrderReadModel>(cached);
            }

            Console.WriteLine($"🧠 [Redis] Cache miss for {id}, querying fallback...");

            var order = await fallback.GetByIdAsync(id, cancellationToken);
            if (order is not null)
            {
                var json = JsonConvert.SerializeObject(order);
                await cache.SetStringAsync(cacheKey, json, cancellationToken);
                Console.WriteLine($"🧠 [Redis] Cached order {id} after fallback");
            }

            return order;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [Redis] Error retrieving order {id}: {ex.Message}");
            return await fallback.GetByIdAsync(id, cancellationToken);
        }
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken)
    {
        Console.WriteLine($"📥 Projecting order {order.Id} to MongoDB...");
        await fallback.ProjectAsync(order, cancellationToken);
    }
}