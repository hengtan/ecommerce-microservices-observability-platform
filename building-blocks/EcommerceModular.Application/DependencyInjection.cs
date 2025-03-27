using System.Reflection;
using EcommerceModular.Application.Strategies.OrderTotal;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceModular.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<IOrderTotalStrategy, NormalStrategy>();
        services.AddScoped<NormalStrategy>();
        services.AddScoped<DiscountStrategy>();
        services.AddScoped<PremiumStrategy>();
        services.AddScoped<OrderTotalStrategySelector>();
        
        return services;
    }
}