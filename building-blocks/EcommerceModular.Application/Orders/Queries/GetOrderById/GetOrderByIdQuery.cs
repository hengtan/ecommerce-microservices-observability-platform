using EcommerceModular.Application.Orders.Projections;
using MediatR;

namespace EcommerceModular.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderReadModel?>;