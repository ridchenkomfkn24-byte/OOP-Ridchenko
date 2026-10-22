using System;
using System.Collections.Generic;

namespace lab13v1
{
    public static class FunctionalUtils
    {
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate initialValue, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));

            TAccumulate result = initialValue;
            foreach (var item in list)
            {
                result = accumulator(result, item);
            }
            return result;
        }

        public static void ForEach<T>(List<T> list, Action<T> action)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in list)
            {
                action(item);
            }
        }

        public static bool Any<T>(List<T> list, Func<T, bool> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} ({Category}) - {Price:C}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №13: Generic-методи та делегати Func/Action ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            Console.WriteLine("--- 1. ТЕСТУВАННЯ З ТИПОМ INT ---");
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var evenNumbers = FunctionalUtils.Filter(numbers, x => x % 2 == 0);
            Console.Write("Filter (парні числа): ");
            FunctionalUtils.ForEach(evenNumbers, x => Console.Write($"{x} "));
            Console.WriteLine();

            var squared = FunctionalUtils.Map(numbers, x => x * x);
            Console.Write("Map (квадрати чисел): ");
            FunctionalUtils.ForEach(squared, x => Console.Write($"{x} "));
            Console.WriteLine();

            int sum = FunctionalUtils.Reduce(numbers, 0, (acc, x) => acc + x);
            Console.WriteLine($"Reduce (сума елементів): {sum}");

            bool hasNegative = FunctionalUtils.Any(numbers, x => x < 0);
            Console.WriteLine($"Any (чи є від'ємні числа): {hasNegative}\n");

            Console.WriteLine("--- 2. ТЕСТУВАННЯ З ТИПОМ STRING ---");
            List<string> words = new List<string> { "csharp", "dotnet", "generics", "delegate", "lambda" };

            var longWords = FunctionalUtils.Filter(words, w => w.Length > 6);
            Console.Write("Filter (слова довше 6 символів): ");
            FunctionalUtils.ForEach(longWords, w => Console.Write($"'{w}' "));
            Console.WriteLine();

            var upperWords = FunctionalUtils.Map(words, w => w.ToUpper());
            Console.Write("Map (у верхній регістр): ");
            FunctionalUtils.ForEach(upperWords, w => Console.Write($"{w} "));
            Console.WriteLine();

            string sentence = FunctionalUtils.Reduce(words, "C#:", (acc, w) => $"{acc} {w}");
            Console.WriteLine($"Reduce (згортання у рядок): {sentence}\n");

            Console.WriteLine("--- 3. ТЕСТУВАННЯ З КЛАСОМ PRODUCT ---");
            List<Product> products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 32000m, Category = "Електроніка" },
                new Product { Name = "Монітор", Price = 8500m, Category = "Електроніка" },
                new Product { Name = "Миша", Price = 950m, Category = "Аксесуари" },
                new Product { Name = "Клавіатура", Price = 2100m, Category = "Аксесуари" }
            };

            var expensiveProducts = FunctionalUtils.Filter(products, p => p.Price > 5000m);
            Console.WriteLine("Filter (товари дорожчі 5000 грн):");
            FunctionalUtils.ForEach(expensiveProducts, p => Console.WriteLine($"  - {p}"));

            var productNames = FunctionalUtils.Map(products, p => p.Name);
            Console.Write("Map (лише назви товарів): ");
            FunctionalUtils.ForEach(productNames, name => Console.Write($"[{name}] "));
            Console.WriteLine();

            decimal totalPrice = FunctionalUtils.Reduce(products, 0m, (acc, p) => acc + p.Price);
            Console.WriteLine($"Reduce (загальна вартість): {totalPrice:C}");

            bool hasAccessories = FunctionalUtils.Any(products, p => p.Category == "Аксесуари");
            Console.WriteLine($"Any (чи є товари категорії 'Аксесуари'): {hasAccessories}");
        }
    }
}