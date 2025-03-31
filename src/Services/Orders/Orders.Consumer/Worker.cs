using EcommerceModular.Application.Interfaces.Messaging;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IKafkaOrderCreatedConsumer _consumer;

    public Worker(ILogger<Worker> logger, IKafkaOrderCreatedConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] Started at: {time}", DateTimeOffset.Now);

        try
        {
            await _consumer.ConsumeAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[Worker] Stopped gracefully due to cancellation.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Worker] Unexpected error while consuming messages.");
            throw;
        }
        finally
        {
            _logger.LogInformation("[Worker] Exiting at: {time}", DateTimeOffset.Now);
        }
    }
}