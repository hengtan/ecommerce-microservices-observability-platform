using EcommerceModular.Application.Interfaces.ReadModels;
using EcommerceModular.Infrastructure.Persistence;
using EcommerceModular.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceModular.Application.Interfaces.Repositories;
using MongoDB.Driver;

namespace EcommerceModular.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IOrderReadService, OrderReadService>();

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        // MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Mongo");
            return new MongoClient(connectionString);
        });
        
        return services;
    }
}