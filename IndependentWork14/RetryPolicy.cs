using System;
using System.Threading;

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0) throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (initialDelay < TimeSpan.Zero || initialDelay.TotalMilliseconds > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(initialDelay));
            if (!double.IsFinite(jitterFactor) || jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor));

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            ArgumentNullException.ThrowIfNull(operation);
            for (int retryAttempt = 0; ; retryAttempt++)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex) when (retryAttempt < _maxRetries
                    && ex is not OperationCanceledException
                    && (shouldRetry == null || shouldRetry(ex)))
                {
                    double jitter = 1 + Random.Shared.NextDouble() * _jitterFactor * 2 - _jitterFactor;
                    double delayMs = _initialDelay == TimeSpan.Zero ? 0
                        : _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitter;
                    // Межа Thread.Sleep захищає від переповнення дуже великої затримки.
                    TimeSpan delay = TimeSpan.FromMilliseconds(Math.Min(delayMs, int.MaxValue));
                    Console.WriteLine($"Повтор {retryAttempt + 1}/{_maxRetries} через {delay.TotalMilliseconds:F1} мс. Причина: {ex.Message}");
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
