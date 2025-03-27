using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Infrastructure.Cache;
using EcommerceModular.Infrastructure.Configurations;
using EcommerceModular.Infrastructure.Persistence;
using EcommerceModular.Infrastructure.Projections;
using EcommerceModular.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceModular.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL setup
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

        // Redis setup
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        // MongoDB config
        services.Configure<MongoSettings>(options =>
            configuration.GetSection("MongoSettings").Bind(options));

        // Mongo projection as base
        services.AddScoped<OrderReadProjection>();

        // Decorate Mongo projection with Redis cache
        services.AddScoped<IOrderReadProjection>(sp =>
        {
            var cache = sp.GetRequiredService<IDistributedCache>();
            var mongo = sp.GetRequiredService<OrderReadProjection>();
            return new RedisOrderReadProjection(cache, mongo);
        });

        // Repositories, etc.
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}