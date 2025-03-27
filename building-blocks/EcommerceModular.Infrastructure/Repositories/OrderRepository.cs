using EcommerceModular.Application.Interfaces.Repositories;
using EcommerceModular.Domain.Entities;
using EcommerceModular.Infrastructure.Persistence;

namespace EcommerceModular.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}