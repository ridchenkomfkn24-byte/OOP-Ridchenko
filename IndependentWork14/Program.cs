using System;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork14
{
    internal class Program
    {
        private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");

        private static int _dbAttempts = 0;
        private static int _paymentAttempts = 0;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = UkCulture;
            CultureInfo.CurrentUICulture = UkCulture;

            Console.WriteLine("=== Самостійна робота №14: Кейси Polly/Retry: короткий звіт ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            // Сценарій 1: Підключення до БД з експоненційним Retry та Jitter
            await Scenario1_DatabaseConnectionRetry();

            Console.WriteLine("\n------------------------------------------------\n");

            // Сценарій 2: Обробка довготривалих запитів до платіжного шлюзу (Timeout + Retry)
            await Scenario2_PaymentGatewayTimeoutAndRetry();

            Console.WriteLine("\n------------------------------------------------\n");

            // Сценарій 3: Захист сервісу розсилки сповіщень через Circuit Breaker
            await Scenario3_NotificationServiceCircuitBreaker();
        }

        #region Сценарій 1: База даних (Retry Strategy)
        private static async Task Scenario1_DatabaseConnectionRetry()
        {
            Console.WriteLine("--- Сценарій 1: Підключення до БД (Retry + Exponential Backoff + Jitter) ---");

            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<DbException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG-RETRY] Повторна спроба #{args.AttemptNumber + 1} через {args.RetryDelay.TotalSeconds:F2}с через помилку: {args.Outcome.Exception?.Message}");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            try
            {
                string result = await pipeline.ExecuteAsync(async cancellationToken =>
                {
                    return await SimulateDatabaseQueryAsync();
                });

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [SUCCESS] Результат запиту до БД: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [FAIL] Операцію заблоковано після всіх спроб: {ex.Message}");
            }
        }

        private static async Task<string> SimulateDatabaseQueryAsync()
        {
            _dbAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба #{_dbAttempts}: Підключення до бази даних товарів...");

            if (_dbAttempts <= 2)
            {
                throw new DbException("Тимчасова помилка з'єднання: Timeout expired / Connection pool exhausted.");
            }

            await Task.Delay(100);
            return "Отримано 150 записів товарів (Баланс рахунку: 45 000,00 грн)";
        }
        #endregion

        #region Сценарій 2: Платіжний шлюз (Timeout + Retry)
        private static async Task Scenario2_PaymentGatewayTimeoutAndRetry()
        {
            Console.WriteLine("--- Сценарій 2: Обробка платежу (Комбінація Timeout + Retry) ---");

            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder()
                        .Handle<TimeoutRejectedException>()
                        .Handle<InvalidOperationException>(),
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(500),
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG-RETRY] Повторна спроба після затримки/помилки #{args.AttemptNumber + 1}. Причина: {args.Outcome.Exception?.GetType().Name}");
                        return ValueTask.CompletedTask;
                    }
                })
                .AddTimeout(TimeSpan.FromSeconds(1.5))
                .Build();

            try
            {
                decimal amount = 1250.50m;
                bool status = await pipeline.ExecuteAsync(async cancellationToken =>
                {
                    return await ProcessPaymentAsync(amount, cancellationToken);
                });

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [SUCCESS] Транзакцію на суму {amount:N2} грн успішно проведено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [FAIL] Не вдалося провести платіж: {ex.Message}");
            }
        }

        private static async Task<bool> ProcessPaymentAsync(decimal amount, CancellationToken cancellationToken)
        {
            _paymentAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба #{_paymentAttempts}: Запит до банкінгу на списування {amount:N2} грн...");

            if (_paymentAttempts == 1)
            {
                await Task.Delay(2500, cancellationToken);
            }

            await Task.Delay(200, cancellationToken);
            return true;
        }
        #endregion

        #region Сценарій 3: Сервіс сповіщень (Circuit Breaker)
        private static async Task Scenario3_NotificationServiceCircuitBreaker()
        {
            Console.WriteLine("--- Сценарій 3: Сервіс SMS-сповіщень (Circuit Breaker) ---");

            // Замінено HttpCircuitBreakerStrategyOptions на стандартний CircuitBreakerStrategyOptions
            var pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(3),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [CIRCUIT-OPEN] Забагато помилок! Захисник відкрився на {args.BreakDuration.TotalSeconds}с. Запити блокуються.");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [CIRCUIT-CLOSED] Сервіс відновлено! Захисник закрився. Прийом запитів відновлено.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [CIRCUIT-HALF-OPEN] Пробний період. Перевіряємо працездатність сервісу...");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Відправка SMS #{i}...");
                    await pipeline.ExecuteAsync(async cancellationToken =>
                    {
                        await SendSmsAsync(i);
                    });
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [SUCCESS] SMS #{i} успішно доставлено.");
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [BLOCKED] SMS #{i} скасовано: Circuit Breaker у стані OPEN!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] SMS #{i} помилка: {ex.Message}");
                }

                if (i == 3)
                {
                    Console.WriteLine("--- Чекаємо 3.5 секунди для відновлення Circuit Breaker ---");
                    await Task.Delay(3500);
                }
                else
                {
                    await Task.Delay(300);
                }
            }
        }

        private static async Task SendSmsAsync(int requestId)
        {
            if (requestId == 1 || requestId == 2)
            {
                throw new InvalidOperationException("SMS Gateway 503: Service Unavailable.");
            }

            await Task.Delay(50);
        }
        #endregion
    }

    public class DbException : Exception
    {
        public DbException(string message) : base(message) { }
    }
}