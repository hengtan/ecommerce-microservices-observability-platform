using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
}