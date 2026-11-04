using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    public static class Calculations
    {
        public static double HeavyTransform(double value)
        {
            for (int i = 1; i <= 12; i++)
            {
                value = Math.Sqrt(Math.Abs(value) + i) + Math.Sin(value);
            }
            return value;
        }

        public static double RunLinq(List<double> values, Func<double, double> operation)
        {
            return values.Where(value => value >= 0.5).Select(operation).Sum();
        }

        public static double RunPlinq(List<double> values, Func<double, double> operation)
        {
            return values.AsParallel().Where(value => value >= 0.5).Select(operation).Sum();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Console.WriteLine("=== Самостійна робота №13: PLINQ — продуктивність і безпека ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1");
            Console.WriteLine($".NET {Environment.Version}; логічних процесорів: {Environment.ProcessorCount}");
            Console.WriteLine("Release; Random(42); медіана 3 повторів; генерація даних поза вимірюванням.");
            Console.WriteLine("| Елементів | Операція | LINQ, мс | PLINQ, мс | LINQ/PLINQ | Результати |\n|---|---|---|---|---|---|");

            foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
            {
                var random = new Random(42);
                var values = new List<double>(size);
                for (int i = 0; i < size; i++) values.Add(random.NextDouble() * 100);
                Compare(values, "Квадратний корінь", Math.Sqrt);
                Compare(values, "12 математичних перетворень", Calculations.HeavyTransform);
            }
            DemonstrateRaceCondition();
        }

        static void Compare(List<double> values, string name, Func<double, double> operation)
        {
            Calculations.RunLinq(values, operation);
            Calculations.RunPlinq(values, operation);
            var sequentialTimes = new List<double>();
            var parallelTimes = new List<double>();

            for (int run = 0; run < 3; run++)
            {
                double sequential = 0;
                double parallel = 0;
                double Measure(Func<double> query, out double result)
                {
                    var watch = Stopwatch.StartNew();
                    result = query(); // Sum виконує запит повністю.
                    watch.Stop();
                    return watch.Elapsed.TotalMilliseconds;
                }

                // Чергуємо порядок, щоб один спосіб не запускався завжди першим.
                if (run % 2 == 0)
                {
                    sequentialTimes.Add(Measure(() => Calculations.RunLinq(values, operation), out sequential));
                    parallelTimes.Add(Measure(() => Calculations.RunPlinq(values, operation), out parallel));
                }
                else
                {
                    parallelTimes.Add(Measure(() => Calculations.RunPlinq(values, operation), out parallel));
                    sequentialTimes.Add(Measure(() => Calculations.RunLinq(values, operation), out sequential));
                }
                if (Math.Abs(sequential - parallel) > 1e-8 * Math.Max(1, Math.Abs(sequential)))
                {
                    throw new InvalidOperationException("Результати LINQ та PLINQ не збігаються в межах похибки.");
                }
            }
            sequentialTimes.Sort();
            parallelTimes.Sort();
            double linqMs = sequentialTimes[1];
            double plinqMs = parallelTimes[1];
            Console.WriteLine($"| {values.Count} | {name} | {linqMs:F3} | {plinqMs:F3} | {linqMs / plinqMs:F2} | збігаються | ");
        }

        public static void DemonstrateRaceCondition()
        {
            const int count = 100_000;
            int degree = Math.Max(2, Math.Min(4, Environment.ProcessorCount));
            Console.WriteLine($"\n--- Спільний лічильник: очікується {count}; потоків у демонстрації: {degree} ---");
            for (int run = 1; run <= 3; run++)
            {
                int unsafeCount = 0;
                Enumerable.Range(0, count).AsParallel().WithDegreeOfParallelism(degree).ForAll(_ =>
                {
                    // Навмисна помилка: розділяємо читання і запис, збільшуючи вікно гонки.
                    int previous = unsafeCount;
                    Thread.SpinWait(20);
                    unsafeCount = previous + 1;
                });
                Console.WriteLine($"Без синхронізації, запуск {run}: {unsafeCount}");
            }

            int safeCount = 0;
            Enumerable.Range(0, count).AsParallel().WithDegreeOfParallelism(degree)
                .ForAll(_ => Interlocked.Increment(ref safeCount));
            int sumCount = Enumerable.Range(0, count).AsParallel().Select(_ => 1).Sum();
            Console.WriteLine($"Interlocked.Increment: {safeCount}");
            Console.WriteLine($"Sum без спільної змінної: {sumCount}");
            if (safeCount != count || sumCount != count)
            {
                throw new InvalidOperationException("Помилка перевірки потокобезпечного підрахунку.");
            }
        }
    }
}
