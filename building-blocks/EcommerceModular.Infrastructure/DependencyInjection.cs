using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Interfaces.ReadModels;
using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Infrastructure.Configurations;
using EcommerceModular.Infrastructure.Messaging;
using EcommerceModular.Infrastructure.Persistence;
using EcommerceModular.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderReadProjection = EcommerceModular.Infrastructure.Persistence.OrderReadProjection;
   
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

        // MongoDB Client
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("Mongo");
            return new MongoClient(connectionString);
        });

        // Mongo Settings
        services.Configure<MongoSettings>(options =>
            configuration.GetSection("MongoSettings").Bind(options));

        // Kafka
        services.AddSingleton<IEventProducer, KafkaEventProducer>();

        // Projection
        services.AddScoped<IOrderReadProjection, OrderReadProjection>();

        return services;
    }
}