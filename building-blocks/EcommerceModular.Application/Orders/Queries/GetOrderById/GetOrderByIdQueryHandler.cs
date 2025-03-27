using EcommerceModular.Application.Interfaces.ReadModels;
using EcommerceModular.Domain.Entities;
using MediatR;

namespace EcommerceModular.Application.Orders.Queries.GetOrderById;


public class GetOrderByIdQueryHandler(IOrderReadService readService) : IRequestHandler<GetOrderByIdQuery, Order?>
{
    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await readService.GetOrderByIdAsync(request.OrderId, cancellationToken);
    }
}