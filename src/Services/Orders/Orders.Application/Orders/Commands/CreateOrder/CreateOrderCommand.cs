using MediatR;
using Orders.Application.DTOs;

namespace Orders.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public string CustomerId { get; set; } = string.Empty;
    public AddressDto ShippingAddress { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
}