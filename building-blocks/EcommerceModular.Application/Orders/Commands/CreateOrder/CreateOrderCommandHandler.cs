using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Application.Strategies.OrderTotal;
using EcommerceModular.Domain.Entities;
using MediatR;

namespace EcommerceModular.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    OrderTotalStrategySelector strategySelector)
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

        // Use the Strategy Pattern to calculate the total
        var strategy = strategySelector.SelectStrategy(request.CustomerType);
        var total = strategy.CalculateTotal(order);

        Console.WriteLine($"[OrderTotal] CustomerType: {request.CustomerType}, Calculated Total: {total:C}");

        await orderRepository.AddAsync(order);

        return order.Id;
    }
}