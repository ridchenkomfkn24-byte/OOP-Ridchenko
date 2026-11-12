using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace lab17v1
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (retryCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryCount), "Кількість повторних спроб не може бути від'ємною.");
            }

            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(400);
            }

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    int currentTry = retryAttempt + 1;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [СПРОБА #{currentTry}] Виконання операції...");
                    return operation();
                }
                catch (Exception ex)
                {
                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ВІДМОВА ПОВТОРУ] Виняток {ex.GetType().Name} не підлягає повторній спробі: {ex.Message}");
                        throw;
                    }

                    if (retryAttempt >= retryCount)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ВИЧЕРПАНО СПРОБИ] Досягнуто ліміт ({retryCount} повторів). Прокидаємо виняток далі.");
                        throw;
                    }

                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt);
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ПОМИЛКА] {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ОЧІКУВАННЯ] Повтор #{retryAttempt + 1} через {delay.TotalSeconds:F2} с...");

                    Thread.Sleep(delay);
                    retryAttempt++;
                }
            }
        }
    }

    public class FileProcessor
    {
        private int _attemptCounter = 0;

        public string ReadFile(string path)
        {
            if (string.Equals(path, "secret.txt", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Доступ до файлу '{path}' заборонено політикою безпеки.");
            }

            _attemptCounter++;

            if (_attemptCounter <= 2)
            {
                throw new FileNotFoundException($"Файл '{path}' тимчасово заблокований або ще не створений (невдала спроба #{_attemptCounter}).", path);
            }

            return $"Вміст файлу '{path}': Фінансовий звіт за квартал — прибуток становить 148 750,00 грн.";
        }
    }

    public class NetworkClient
    {
        private int _attemptCounter = 0;

        public string DownloadData(string url)
        {
            _attemptCounter++;

            if (_attemptCounter <= 3)
            {
                throw new HttpRequestException($"Помилка з'єднання з сервером '{url}' (503 Service Unavailable, невдала спроба #{_attemptCounter}).");
            }

            return $"Відповідь від '{url}': {{ \"status\": \"OK\", \"balance\": \"62 400,50 грн\" }}";
        }
    }

    internal class Program
    {
        private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = UkCulture;
            CultureInfo.CurrentUICulture = UkCulture;

            Console.WriteLine("=== Лабораторна робота №17: Обробка IO/мережевих помилок та патерн Retry ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант: 1\n");

            Func<Exception, bool> shouldRetryPolicy = ex =>
                ex is FileNotFoundException || ex is HttpRequestException;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            // Сценарій 1: Читання файлу (2 невдачі FileNotFoundException -> успіх)
            Console.WriteLine("--- Сценарій 1: FileProcessor.ReadFile (успіх на 3-й спробі) ---");
            try
            {
                string fileContent = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadFile("report_2026.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: shouldRetryPolicy
                );

                Console.WriteLine($"[УСПІХ] {fileContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧНА ПОМИЛКА] Не вдалося прочитати файл: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("[FINALLY] Завершено блок роботи з файловим потоком.\n");
            }

            Console.WriteLine("-----------------------------------------------------------------\n");

            // Сценарій 2: Завантаження з мережі (3 невдачі HttpRequestException -> успіх)
            Console.WriteLine("--- Сценарій 2: NetworkClient.DownloadData (успіх на 4-й спробі) ---");
            try
            {
                string downloadedData = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.DownloadData("https://api.bank.ua/v1/account"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: shouldRetryPolicy
                );

                Console.WriteLine($"[УСПІХ] {downloadedData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧНА ПОМИЛКА] Не вдалося завантажити дані: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("[FINALLY] Мережеве з'єднання закрито, ресурси звільнено.\n");
            }

            Console.WriteLine("-----------------------------------------------------------------\n");

            // Сценарій 3: Вибіркова обробка через shouldRetry (неповторюваний виняток)
            Console.WriteLine("--- Сценарій 3: Перевірка shouldRetry (UnauthorizedAccessException без повторів) ---");
            try
            {
                string secretContent = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadFile("secret.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: shouldRetryPolicy
                );

                Console.WriteLine($"[УСПІХ] {secretContent}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[ПЕРЕХОПЛЕНО В MAIN] Повторні спроби не виконувались, оскільки виняток не є тимчасовим: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("[FINALLY] Перевірку вибіркової обробки винятків завершено.");
            }
        }
    }
}