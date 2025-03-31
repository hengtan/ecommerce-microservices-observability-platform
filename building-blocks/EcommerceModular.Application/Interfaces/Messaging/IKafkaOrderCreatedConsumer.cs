namespace EcommerceModular.Application.Interfaces.Messaging
{
    public interface IKafkaOrderCreatedConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}