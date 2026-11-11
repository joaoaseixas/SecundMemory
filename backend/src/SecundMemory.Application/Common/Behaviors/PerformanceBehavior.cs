using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "{RequestName} took {ElapsedMs}ms (threshold {ThresholdMs}ms)",
                typeof(TRequest).Name, stopwatch.ElapsedMilliseconds, SlowRequestThresholdMs);
        }

        return response;
    }
}
