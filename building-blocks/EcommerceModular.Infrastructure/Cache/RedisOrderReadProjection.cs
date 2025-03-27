using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Models;
using EcommerceModular.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;


namespace EcommerceModular.Infrastructure.Cache;

public class RedisOrderReadProjection(IDistributedCache cache, IOrderReadProjection inner) : IOrderReadProjection
{
    // fallback: Mongo

    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };

    public async Task<ProjectedOrder?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetStringAsync(GetKey(orderId), cancellationToken);
        if (cached != null)
            return JsonConvert.DeserializeObject<ProjectedOrder>(cached);

        var fromDb = await inner.GetByIdAsync(orderId, cancellationToken);
        if (fromDb != null)
        {
            var serialized = JsonConvert.SerializeObject(fromDb);
            await cache.SetStringAsync(GetKey(orderId), serialized, CacheOptions, cancellationToken);
        }

        return fromDb;
    }

    public async Task ProjectAsync(Order order, CancellationToken cancellationToken = default)
    {
        await inner.ProjectAsync(order, cancellationToken); // always project to Mongo
        var projection = new ProjectedOrder
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Total = order.Total,
            Status = order.Status.ToString()
        };

        var serialized = JsonConvert.SerializeObject(projection);
        await cache.SetStringAsync(GetKey(order.Id), serialized, CacheOptions, cancellationToken);
    }

    private static string GetKey(Guid orderId) => $"order:{orderId}";
}