using System.Text.Json;
using Confluent.Kafka;
using EcommerceModular.Domain.Events;
using MongoDB.Driver;

namespace Orders.KafkaConsumer;


public class KafkaConsumerWorker : BackgroundService
{
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private readonly IMongoCollection<OrderCreatedEvent> _mongo;

    public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger, IConfiguration config)
    {
        _logger = logger;

        var mongoClient = new MongoClient(config["Mongo"]);
        _mongo = mongoClient
            .GetDatabase("orders_read")
            .GetCollection<OrderCreatedEvent>("order_events");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "orders-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("orders.created");

        _logger.LogInformation("Kafka consumer started...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var message = result.Message.Value;

                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);
                if (orderEvent != null)
                {
                    await _mongo.InsertOneAsync(orderEvent, cancellationToken: stoppingToken);

                    _logger.LogInformation($"Consumed and stored OrderCreatedEvent: {orderEvent.OrderId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka message");
            }
        }
    }
}