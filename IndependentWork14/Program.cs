using System;
using System.IO;

namespace IndependentWork14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №15: Exponential backoff із jitter ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1");
            var policy = new RetryPolicy(3, TimeSpan.FromMilliseconds(40), 0.2);
            int attempts = 0;

            Console.WriteLine("\n--- 1. Тимчасова помилка, потім успіх ---");
            string result = policy.Execute(() =>
            {
                attempts++;
                Console.WriteLine($"Читання файлу, спроба {attempts}");
                if (attempts <= 2) throw new IOException("Файл тимчасово зайнятий.");
                return "Файл прочитано.";
            }, ex => ex is IOException);
            Console.WriteLine(result);

            Console.WriteLine("\n--- 2. Помилка залишається після всіх повторів ---");
            var shortPolicy = new RetryPolicy(2, TimeSpan.FromMilliseconds(30), 0.2);
            attempts = 0;
            try
            {
                shortPolicy.Execute<string>(() =>
                {
                    attempts++;
                    Console.WriteLine($"З’єднання, спроба {attempts}");
                    throw new TimeoutException("Сервіс не відповідає.");
                }, ex => ex is TimeoutException);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Завершено після {attempts} спроб: {ex.Message}");
            }

            Console.WriteLine("\n--- 3. Постійна помилка: shouldRetry забороняє повтор ---");
            attempts = 0;
            try
            {
                policy.Execute<int>(() =>
                {
                    attempts++;
                    throw new ArgumentException("Некоректний ідентифікатор.");
                }, ex => ex is IOException);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Спроб: {attempts}. Без повтору: {ex.Message}");
            }
        }
    }
}
