using System;
using System.Collections.Generic;
using System.Globalization;

namespace IndependentWork7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
            CultureInfo.CurrentUICulture = new CultureInfo("uk-UA");
            Console.WriteLine("=== Самостійна робота №8: Узагальнений кеш із політикою FIFO ===\n");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            Cache<string, decimal> cache = new Cache<string, decimal>(3);
            Console.WriteLine("--- Додавання товарів у кеш (ємність — 3) ---");
            cache.Add("Ноутбук", 45000.00m);
            cache.Add("Смартфон", 22500.50m);
            cache.Add("Навушники", 3200.00m);

            Console.WriteLine($"Кількість товарів у кеші: {cache.Count}");
            Console.WriteLine($"Ціна ноутбука: {cache.Get("Ноутбук"):N2} грн");
            Console.WriteLine($"Ціна смартфона: {cache.Get("Смартфон"):N2} грн");

            Console.WriteLine("\n--- Переповнення кешу: додаємо монітор ---");
            cache.Add("Монітор", 11800.00m);
            Console.WriteLine($"Ноутбук є в кеші: {(cache.ContainsKey("Ноутбук") ? "Так" : "Ні")}");
            Console.WriteLine($"Смартфон є в кеші: {(cache.ContainsKey("Смартфон") ? "Так" : "Ні")}");
            Console.WriteLine($"Ціна монітора: {cache.Get("Монітор"):N2} грн");
            Console.WriteLine($"Кількість товарів після переповнення: {cache.Count}");
            Console.WriteLine("За правилом FIFO видалено ноутбук — його додали першим.");

            Console.WriteLine("\n--- Оновлення ціни смартфона ---");
            cache.Add("Смартфон", 21000.00m);
            Console.WriteLine($"Нова ціна смартфона: {cache.Get("Смартфон"):N2} грн");
            Console.WriteLine($"Кількість товарів після оновлення: {cache.Count}");

            try
            {
                Console.WriteLine("\n--- Спроба отримати видалений товар «Ноутбук» ---");
                cache.Get("Ноутбук");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[ОЧІКУВАНИЙ ВИНЯТОК]: {ex.Message}");
            }
        }
    }
}
