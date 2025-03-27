using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public class DiscountStrategy : IOrderTotalStrategy
{
    public decimal CalculateTotal(Order order)
    {
        var total = order.Items.Sum(item => item.Total);
        return total * 0.9m; // 10% discount
    }
}