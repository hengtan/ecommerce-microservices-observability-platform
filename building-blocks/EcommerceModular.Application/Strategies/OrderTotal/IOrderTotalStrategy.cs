using EcommerceModular.Domain.Entities;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public interface IOrderTotalStrategy
{
    decimal CalculateTotal(Order order);
}