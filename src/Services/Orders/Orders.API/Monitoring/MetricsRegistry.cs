using Prometheus;

namespace Orders.API.Monitoring;

public static class MetricsRegistry
{
    // Counter for total number of created orders
    public static readonly Counter OrdersCreated = Metrics
        .CreateCounter("app_orders_created_total", "Total number of orders created.");

    // Histogram for measuring duration of order creation (in seconds)
    public static readonly Histogram OrderProcessingDuration = Metrics
        .CreateHistogram("app_order_processing_duration_seconds", 
            "Order processing duration in seconds.");
}