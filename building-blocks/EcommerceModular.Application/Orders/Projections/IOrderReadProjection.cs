using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Orders.Projections;

public interface IOrderReadProjection
{
    Task ProjectAsync(Order order, CancellationToken cancellationToken);
}