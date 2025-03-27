using EcommerceModular.Application.Policies;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Domain.Models.ReadModels;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Polly;
using IOrderReadProjection = EcommerceModular.Application.Interfaces.Persistence.IOrderReadProjection;


namespace EcommerceModular.Infrastructure.Cache;
public class RedisOrderReadProjection(IDistributedCache cache, IOrderReadProjection fallback) : IOrderReadProjection
{
    public async Task<ProjectedOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";

        var retryPolicy = PollyPolicies.GetRetryPolicy<ProjectedOrder?>();
        var circuitBreaker = PollyPolicies.GetCircuitBreakerPolicy<ProjectedOrder?>();

        return await retryPolicy.WrapAsync(circuitBreaker)
            .ExecuteAsync(async () =>
            {
                // 🔍 Try Redis first
                var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
                if (!string.IsNullOrEmpty(cached))
                {
                    return JsonConvert.DeserializeObject<ProjectedOrder>(cached);
                }

                // 🔄 Fallback to MongoDB (decorated service)
                var order = await fallback.GetByIdAsync(id, cancellationToken);

                if (order != null)
                {
                    var json = JsonConvert.SerializeObject(order);
                    await cache.SetStringAsync(cacheKey, json, cancellationToken);
                }

                return order;
            });
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken)
    {
        await fallback.ProjectAsync(order, cancellationToken);

        var projected = await fallback.GetByIdAsync(order.Id, cancellationToken);
        if (projected != null)
        {
            var json = JsonConvert.SerializeObject(projected);
            var cacheKey = $"order:{projected.Id}";
            await cache.SetStringAsync(cacheKey, json, cancellationToken);
        }
    }
}