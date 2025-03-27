using Orders.KafkaConsumer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<KafkaConsumerWorker>();
    })
    .Build();

await host.RunAsync();