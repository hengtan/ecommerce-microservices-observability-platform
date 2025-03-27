namespace EcommerceModular.Application.Interfaces.Messaging;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T message);
}