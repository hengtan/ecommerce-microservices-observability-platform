using EcommerceModular.Application.Common.Metrics;
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
    IOrderMetrics metrics) // ✅ Injeta métrica
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var timer = metrics.MeasureOrderProcessingDuration(); // ⏱️ inicia o cronômetro

        var order = BuildOrderFromRequest(request);

        var total = CalculateTotal(order, request.CustomerType);

        // Log opcional (pode trocar por Serilog depois)
        Console.WriteLine($"[OrderTotal] CustomerType: {request.CustomerType}, Calculated Total: {total:C}");

        await PersistOrderAsync(order);
        await PublishOrderCreatedEventAsync(order);

        metrics.IncrementOrdersCreated(); // 📈 incrementa métrica

        return order.Id;
    }

    private static Order BuildOrderFromRequest(CreateOrderCommand request)
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

        return new Order(request.CustomerId, address, items);
    }

    private decimal CalculateTotal(Order order, string customerType)
    {
        var strategy = strategySelector.SelectStrategy(customerType);
        return strategy.CalculateTotal(order);
    }

    private Task PersistOrderAsync(Order order)
    {
        return orderRepository.AddAsync(order);
    }

    private Task PublishOrderCreatedEventAsync(Order order)
    {
        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            order.CreatedAt,
            new ShippingAddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                Country = order.ShippingAddress.Country,
                ZipCode = order.ShippingAddress.ZipCode
            },
            order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity
            }).ToList()
        );
        
        return eventProducer.ProduceAsync("orders.created", orderCreatedEvent);
    }
}