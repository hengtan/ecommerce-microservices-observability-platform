using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public class NormalStrategy : IOrderTotalStrategy
{
    public decimal CalculateTotal(Order order)
    {
        return order.Items.Sum(item => item.Total);
    }
}