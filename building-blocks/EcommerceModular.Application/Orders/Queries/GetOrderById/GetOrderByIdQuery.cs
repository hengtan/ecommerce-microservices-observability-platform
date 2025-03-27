using EcommerceModular.Domain.Entities;
using MediatR;

namespace EcommerceModular.Application.Orders.Queries.GetOrderById;


public record GetOrderByIdQuery(Guid OrderId) : IRequest<Order?>;