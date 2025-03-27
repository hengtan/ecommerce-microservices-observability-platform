using EcommerceModular.Application.Common.Metrics;
using Prometheus;

namespace Orders.API.Monitoring;

public class PrometheusOrderMetrics : IOrderMetrics
{
    public void IncrementOrdersCreated()
    {
        MetricsRegistry.OrdersCreated.Inc();
    }

    public IDisposable MeasureOrderProcessingDuration()
    {
        return MetricsRegistry.OrderProcessingDuration.NewTimer();
    }
}