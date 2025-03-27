using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Domain.Entities;
using MediatR;

namespace EcommerceModular.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

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

        await _orderRepository.AddAsync(order);

        return order.Id;
    }
}