using Confluent.Kafka;
using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Domain.Entities;
using Newtonsoft.Json;
using Orders.Consumer.Dtos;

namespace Orders.Consumer;

public class KafkaOrderCreatedConsumer(
    ILogger<KafkaOrderCreatedConsumer> logger,
    IOrderReadProjection projection,
    IConfiguration configuration)
    : IKafkaOrderCreatedConsumer
{
    private readonly ConsumerConfig _config = new()
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"],
        GroupId = configuration["Kafka:GroupId"] ?? "orders-consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = true
    };

    private readonly string _topic = configuration["Kafka:Topic"] ?? "orders.created";

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("🔥 Kafka consumer started");

        using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        consumer.Subscribe(_topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(cancellationToken);
                Console.WriteLine($"✅ Received: {consumeResult.Message.Value}");

                var dto = JsonConvert.DeserializeObject<OrderDto>(consumeResult.Message.Value);

                if (dto is null)
                {
                    Console.WriteLine("❌ Error deserializing JSON into OrderDto");
                    return;
                }

                // Cria o domínio Order a partir do DTO
                var order = new Order(
                    customerId: dto.CustomerId.ToString(), // 👈 Aqui converte Guid para string
                    shippingAddress: new Address(
                        dto.ShippingAddress.Street,
                        dto.ShippingAddress.City,
                        dto.ShippingAddress.State,
                        dto.ShippingAddress.Country,
                        dto.ShippingAddress.ZipCode
                    ),
                    items: dto.Items.Select(i =>
                            new OrderItem(i.ProductId, i.ProductName, i.Quantity,
                                0) // Aqui o preço pode ser 0 por enquanto
                    ).ToList()
                );

                // Seta manualmente o OrderId, pois o construtor gera um novo
                typeof(Order).GetProperty("Id")!.SetValue(order, dto.OrderId);

                Console.WriteLine($"📦 Parsed order: {order.Id}");

                await projection.ProjectAsync(order, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        consumer.Close();
    }
}