using Microsoft.Extensions.DependencyInjection;

namespace EcommerceModular.Application.Strategies.OrderTotal;

public class OrderTotalStrategySelector(IServiceProvider serviceProvider)
{
    public IOrderTotalStrategy SelectStrategy(string customerType)
    {
        return customerType switch
        {
            "Premium" => serviceProvider.GetRequiredService<PremiumStrategy>(),
            "Discount" => serviceProvider.GetRequiredService<DiscountStrategy>(),
            _ => serviceProvider.GetRequiredService<NormalStrategy>()
        };
    }
}