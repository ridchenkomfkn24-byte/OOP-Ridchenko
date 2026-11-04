using System;
using System.Collections.Generic;
using System.Globalization;

namespace CollectionUtilsDemo
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
            Console.WriteLine("=== Самостійна робота №10: Делегати й лямбда-вирази ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            var numbers = new List<int> { 4, -2, 7, 10, 3 };
            Console.WriteLine("--- Колекція чисел ---");
            Console.WriteLine("Квадрати: " + string.Join(", ", CollectionUtils.Map(numbers, n => n * n)));
            Console.WriteLine("Парні: " + string.Join(", ", CollectionUtils.Filter(numbers, n => n % 2 == 0)));
            Console.WriteLine("Максимальне число: " + CollectionUtils.MaxBy(numbers, n => n));
            int sum = 0;
            CollectionUtils.ForEach(numbers, n => sum += n);
            Console.WriteLine($"Сума через ForEach: {sum}");

            var words = new List<string> { "кеш", "словник", "множина", "делегат" };
            Console.WriteLine("\n--- Колекція рядків ---");
            Console.WriteLine("Верхній регістр: " + string.Join(", ", CollectionUtils.Map(words, w => w.ToUpperInvariant())));
            Console.WriteLine("Довші за 4 символи: " + string.Join(", ", CollectionUtils.Filter(words, w => w.Length > 4)));
            Console.WriteLine("Найдовше слово: " + CollectionUtils.MaxBy(words, w => w.Length));
            CollectionUtils.ForEach(words, w => Console.WriteLine($"{w}: {w.Length} символів"));

            var products = new List<Product>
            {
                new Product { Name = "Монітор", Price = 8500m },
                new Product { Name = "Миша", Price = 950m },
                new Product { Name = "Клавіатура", Price = 2100m }
            };
            Console.WriteLine("\n--- Колекція товарів ---");
            Console.WriteLine("Назви: " + string.Join(", ", CollectionUtils.Map(products, p => p.Name)));
            Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий товар: {mostExpensive.Name}, {mostExpensive.Price:N2} грн");
            Console.WriteLine("Товари дорожчі за 1000 грн:");
            CollectionUtils.ForEach(CollectionUtils.Filter(products, p => p.Price > 1000m),
                p => Console.WriteLine($"  {p.Name}: {p.Price:N2} грн"));
            Console.WriteLine("Усі товари:");
            CollectionUtils.ForEach(products, p => Console.WriteLine($"  {p.Name}: {p.Price:N2} грн"));
        }
    }
}
