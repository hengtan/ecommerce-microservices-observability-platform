using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Interfaces.ReadModels;

public interface IOrderReadService
{
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
}