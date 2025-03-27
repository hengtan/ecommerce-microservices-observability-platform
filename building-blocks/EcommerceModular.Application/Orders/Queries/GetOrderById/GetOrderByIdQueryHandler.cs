using EcommerceModular.Application.Interfaces.ReadModels;
using EcommerceModular.Application.Orders.Projections;
using MediatR;

namespace EcommerceModular.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderReadService readService)
    : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
{
    public async Task<OrderReadModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await readService.GetOrderByIdAsync(request.OrderId, cancellationToken);
    }
}