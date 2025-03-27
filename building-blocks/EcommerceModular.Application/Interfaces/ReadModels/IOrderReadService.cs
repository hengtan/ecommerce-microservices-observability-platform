using EcommerceModular.Application.Orders.Projections;

namespace EcommerceModular.Application.Interfaces.ReadModels;

public interface IOrderReadService
{
    Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
}