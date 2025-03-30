using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Infrastructure.Persistence;

namespace EcommerceModular.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
    }
}