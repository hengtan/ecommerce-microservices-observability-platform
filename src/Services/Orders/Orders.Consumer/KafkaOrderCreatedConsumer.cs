using Confluent.Kafka;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Domain.Entities;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Orders.Consumer;

public class KafkaOrderCreatedConsumer(ILogger<KafkaOrderCreatedConsumer> logger, IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly string _topic = "orders.created";
    private readonly string _groupId = "orders-consumer-group";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        logger.LogInformation("Kafka consumer started, listening on topic: {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var message = result.Message.Value;

                logger.LogInformation("Received message: {Message}", message);

                var order = JsonConvert.DeserializeObject<Order>(message);

                if (order is not null)
                {
                    using var scope = serviceProvider.CreateScope();
                    var projection = scope.ServiceProvider.GetRequiredService<IOrderReadProjection>();

                    await projection.ProjectAsync(order, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Kafka message");
            }
        }

        consumer.Close();
    }
}