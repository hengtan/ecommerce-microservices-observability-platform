using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Domain.Models.ReadModels;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;


namespace EcommerceModular.Infrastructure.Cache;
public class RedisOrderReadProjection(IDistributedCache cache, IOrderReadProjection fallback) : IOrderReadProjection
{
    public async Task<ProjectedOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{id}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonConvert.DeserializeObject<ProjectedOrder>(cached);
        }

        var order = await fallback.GetByIdAsync(id, cancellationToken);

        if (order != null)
        {
            var json = JsonConvert.SerializeObject(order);
            await cache.SetStringAsync(cacheKey, json, cancellationToken);
        }

        return order;
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