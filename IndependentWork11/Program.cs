using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace IndependentWork11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
            Console.WriteLine("=== Самостійна робота №12: LINQ pipelines — ETL та аналітика ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1");
            SalesData data = DataGenerator.Create();
            Console.WriteLine($"Товарів: {data.Products.Count}; клієнтів: {data.Customers.Count}; продажів: {data.Sales.Count}");

            Measure("1. Найпопулярніші продукти за категоріями", () => SalesAnalytics.TopProductsByCategory(data),
                row => Console.WriteLine($"{row.Category}: {row.Name} — {row.Quantity} шт."));
            Measure("2. Дохід за містами", () => SalesAnalytics.RevenueByCity(data),
                row => Console.WriteLine($"{row.City}: {row.Revenue:N2} грн"));
            Measure("3. Продажі за місяцями та роками", () => SalesAnalytics.MonthlySales(data),
                row => Console.WriteLine($"{row.Year}-{row.Month:D2}: {row.Quantity} шт., {row.Revenue:N2} грн"));
            Measure("4. П’ять найбільших покупців із 01.01.2026", () => SalesAnalytics.TopCustomers(data, new DateTime(2026, 1, 1)),
                row => Console.WriteLine($"{row.Name}: {row.Revenue:N2} грн"));
        }

        static void Measure<T>(string title, Func<List<T>> query, Action<T> print)
        {
            query(); // Прогрів перед вимірюваннями.
            var watch = new Stopwatch();
            List<T> result = new List<T>();
            const int repeats = 20;
            watch.Start();
            for (int i = 0; i < repeats; i++) result = query();
            watch.Stop();
            Console.WriteLine($"\n--- {title} ---");
            foreach (T row in result) print(row);
            Console.WriteLine($"Середній час (20 повторів): {watch.Elapsed.TotalMilliseconds / repeats:F4} мс");
        }
    }
}
