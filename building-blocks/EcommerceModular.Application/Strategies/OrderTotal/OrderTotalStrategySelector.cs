using Microsoft.Extensions.DependencyInjection;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public class OrderTotalStrategySelector(IServiceProvider serviceProvider)
{
    public IOrderTotalStrategy SelectStrategy(string customerType)
    {
        return customerType switch
        {
            "Normal" => (IOrderTotalStrategy) serviceProvider.GetRequiredService<NormalStrategy>(),
            "Discount" => (IOrderTotalStrategy) serviceProvider.GetRequiredService<DiscountStrategy>(),
            "Premium" => (IOrderTotalStrategy) serviceProvider.GetRequiredService<PremiumStrategy>(),
            _ => throw new ArgumentException($"Invalid customer type: {customerType}")
        };
    }
}