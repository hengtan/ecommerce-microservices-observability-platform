using Polly;

namespace EcommerceModular.Application.Policies;

public static class PollyPolicies
{
    public static IAsyncPolicy<T> GetRetryPolicy<T>() =>
        Policy<T>
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * attempt)
            );

    public static IAsyncPolicy<T> GetCircuitBreakerPolicy<T>() =>
        Policy<T>
            .Handle<Exception>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(10)
            );
}