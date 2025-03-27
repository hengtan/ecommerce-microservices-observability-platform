using EcommerceModular.Application.Interfaces.Messaging;
using EcommerceModular.Application.Interfaces.Persistence;
using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Application.Strategies.OrderTotal;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Domain.Events;
using MediatR;

namespace EcommerceModular.Application.Orders.Commands.CreateOrder;


public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    OrderTotalStrategySelector strategySelector,
    IEventProducer eventProducer,
    IOrderReadProjection readProjection)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.Country,
            request.ShippingAddress.ZipCode
        );

        var items = request.Items.Select(i =>
            new OrderItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList();

        var order = new Order(request.CustomerId, address, items);

        // Strategy Pattern – Calculate total based on CustomerType
        var strategy = strategySelector.SelectStrategy(request.CustomerType);
        var total = strategy.CalculateTotal(order);

        Console.WriteLine($"[OrderTotal] CustomerType: {request.CustomerType}, Calculated Total: {total:C}");

        // Save to PostgreSQL (command side)
        await orderRepository.AddAsync(order);

        // Project to MongoDB + write to file (read side)
        await readProjection.ProjectAsync(order, cancellationToken);

        // Publish OrderCreatedEvent to Kafka
        var orderCreatedEvent = new OrderCreatedEvent(order.Id, order.CustomerId, order.CreatedAt);
        await eventProducer.ProduceAsync("orders.created", orderCreatedEvent);

        return order.Id;
    }
}