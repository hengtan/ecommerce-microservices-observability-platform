using Orders.Consumer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add infrastructure (Mongo, Redis, Kafka, etc)

        // Kafka Consumer config
        services.AddHostedService<KafkaOrderCreatedConsumer>();
    })
    .Build();

await host.RunAsync();