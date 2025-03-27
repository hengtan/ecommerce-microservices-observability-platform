using System.Text.Json;
using Confluent.Kafka;
using EcommerceModular.Application.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;

namespace EcommerceModular.Infrastructure.Messaging;

public class KafkaEventProducer : IEventProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaEventProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
    }
}