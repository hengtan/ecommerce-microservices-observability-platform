using Orders.Domain.Entities;

namespace Orders.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
}