using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public class PremiumStrategy : IOrderTotalStrategy
{
    public decimal CalculateTotal(Order order)
    {
        var total = order.Items.Sum(item => item.Total);
        return total + 20.00m; // Add flat premium shipping fee
    }
}