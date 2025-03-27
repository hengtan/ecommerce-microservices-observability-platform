using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Interfaces.Persistence;

public interface IOrderReadProjection
{
    Task ProjectAsync(Order order, CancellationToken cancellationToken = default);
}