using EcommerceModular.Domain.Entities;
using EcommerceModular.Domain.Models.ReadModels;

namespace EcommerceModular.Application.Interfaces.Persistence;

public interface IOrderReadProjection
{
    Task ProjectAsync(Order order, CancellationToken cancellationToken = default);
    Task<ProjectedOrder?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken); 
}