using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Infrastructure.Cache;
using EcommerceModular.Infrastructure.Configurations;
using EcommerceModular.Infrastructure.Projections;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Orders.Consumer;

var builder = Host.CreateApplicationBuilder(args);

// Register Guid serializers with Standard representation
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard)));

// MongoDB Client configuration
// builder.Services.AddSingleton<IMongoClient>(sp =>
// {
//     var connectionString = sp.GetRequiredService<IOptions<MongoSettings>>().Value.ConnectionString;
//     
//     Console.WriteLine($"✅ Mongo connection string: {connectionString}");
//     
//     return new MongoClient(connectionString);
// });
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<OrderReadProjection>();
builder.Services.AddScoped<IOrderReadProjection>(sp =>
    new RedisOrderReadProjection(
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<OrderReadProjection>())
);

// Redis configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis"];
});

// Main projection (Mongo)
builder.Services.AddScoped<OrderReadProjection>();

// Redis decorator
builder.Services.AddScoped<IOrderReadProjection>(sp =>
    new RedisOrderReadProjection(
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<OrderReadProjection>())
);

// Kafka consumer
builder.Services.AddScoped<IKafkaOrderCreatedConsumer, KafkaOrderCreatedConsumer>();

var adminConfig = new AdminClientConfig
{
    BootstrapServers = "localhost:9092"
};

using var adminClient = new AdminClientBuilder(adminConfig).Build();

try
{
    await adminClient.CreateTopicsAsync(new TopicSpecification[]
    {
        new TopicSpecification
        {
            Name = "orders.created",
            NumPartitions = 1,
            ReplicationFactor = 1
        }
    });
}
catch (CreateTopicsException ex)
{
    if (ex.Results.Any(r => r.Error.Code != ErrorCode.TopicAlreadyExists))
        throw;
}

// Worker
builder.Services.AddHostedService<Worker>();

// Run the service
var host = builder.Build();
host.Run();