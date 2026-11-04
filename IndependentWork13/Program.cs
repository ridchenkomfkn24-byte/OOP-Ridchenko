using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №14: Кейси Polly/Retry ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1");
            RetryQueue();
            await CircuitBreakerDatabase();
            await RetryWithTimeout();
        }

        static void RetryQueue()
        {
            Console.WriteLine("\n--- 1. Надсилання повідомлення до перевантаженої черги: Retry ---");
            int attempts = 0;
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(50),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = retry =>
                    {
                        Console.WriteLine($"Повтор {retry.AttemptNumber + 1}, затримка {retry.RetryDelay.TotalMilliseconds:F0} мс: {retry.Outcome.Exception?.Message}");
                        return default;
                    }
                }).Build();

            string result = pipeline.Execute(() =>
            {
                attempts++;
                Console.WriteLine($"Спроба надсилання {attempts}, MessageId = msg-001");
                if (attempts <= 2) throw new IOException("Черга тимчасово перевантажена.");
                return "Повідомлення прийнято.";
            });
            Console.WriteLine(result);
        }

        static async Task CircuitBreakerDatabase()
        {
            Console.WriteLine("\n--- 2. Недоступна база даних: CircuitBreaker ---");
            int calls = 0;
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    FailureRatio = 1.0,
                    MinimumThroughput = 2,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromSeconds(1),
                    OnOpened = _ => { Console.WriteLine("Стан Open: запити до бази тимчасово блокуються."); return default; },
                    OnHalfOpened = _ => { Console.WriteLine("Стан HalfOpen: перевіряємо відновлення бази."); return default; },
                    OnClosed = _ => { Console.WriteLine("Стан Closed: доступ до бази відновлено."); return default; }
                }).Build();

            string ReadDatabase()
            {
                calls++;
                Console.WriteLine($"Фактичний виклик бази №{calls}");
                if (calls <= 2) throw new IOException("База недоступна.");
                return "Дані успішно отримано.";
            }

            for (int request = 1; request <= 3; request++)
            {
                try
                {
                    Console.WriteLine(pipeline.Execute(ReadDatabase));
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("Запит відхилено без виклику бази: коло розімкнене.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }

            await Task.Delay(1100);
            Console.WriteLine(pipeline.Execute(ReadDatabase));
            Console.WriteLine($"Фактичних звернень до бази: {calls} із 4 запитів.");
        }

        static async Task RetryWithTimeout()
        {
            Console.WriteLine("\n--- 3. Завантаження звіту: Retry + Timeout ---");
            int attempts = 0;
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>(),
                    MaxRetryAttempts = 1,
                    Delay = TimeSpan.FromMilliseconds(50),
                    OnRetry = _ => { Console.WriteLine("Повторюємо читання звіту після тайм-ауту."); return default; }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(200),
                    OnTimeout = _ => { Console.WriteLine("Спроба перевищила ліміт 200 мс."); return default; }
                }).Build();

            string result = await pipeline.ExecuteAsync(async cancellationToken =>
            {
                attempts++;
                Console.WriteLine($"Завантаження звіту, спроба {attempts}");
                // Передаємо токен, щоб тайм-аут дійсно припиняв очікування.
                await Task.Delay(attempts == 1 ? 600 : 30, cancellationToken);
                return "Звіт завантажено.";
            }, CancellationToken.None);
            Console.WriteLine(result);
        }
    }
}
