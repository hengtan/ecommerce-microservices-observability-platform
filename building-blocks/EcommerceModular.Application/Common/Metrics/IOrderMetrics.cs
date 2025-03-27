namespace EcommerceModular.Application.Common.Metrics;

public interface IOrderMetrics
{
    void IncrementOrdersCreated();
    IDisposable MeasureOrderProcessingDuration();
}