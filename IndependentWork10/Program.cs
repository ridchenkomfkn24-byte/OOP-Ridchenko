using System;
using System.Collections.Generic;

namespace IndependentWork10
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first) throw new InvalidOperationException("Колекція порожня.");

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Price} грн";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Самостійна робота №10: Делегати й лямбда-вирази ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            // 1. Демонстрація з List<int>
            Console.WriteLine("--- 1. ТЕСТУВАННЯ З ВХІДНОЮ КОЛЕКЦІЄЮ List<int> ---");
            List<int> numbers = new List<int> { 5, 12, 3, 28, 7, 14, 21 };

            var squaredNumbers = CollectionUtils.Map(numbers, x => x * x);
            Console.Write("Map (Квадрати чисел): ");
            CollectionUtils.ForEach(squaredNumbers, n => Console.Write($"{n} "));
            Console.WriteLine();

            int maxNumber = CollectionUtils.MaxBy(numbers, x => x);
            Console.WriteLine($"MaxBy (Максимальне число): {maxNumber}");

            var evenNumbers = CollectionUtils.Filter(numbers, x => x % 2 == 0);
            Console.Write("Filter (Парні числа): ");
            CollectionUtils.ForEach(evenNumbers, n => Console.Write($"{n} "));
            Console.WriteLine("\n");

            // 2. Демонстрація з List<string>
            Console.WriteLine("--- 2. ТЕСТУВАННЯ З ВХІДНОЮ КОЛЕКЦІЄЮ List<string> ---");
            List<string> words = new List<string> { "Apple", "Banana", "Watermelon", "Kiwi", "Orange" };

            var wordLengths = CollectionUtils.Map(words, w => $"{w} ({w.Length} букв)");
            Console.WriteLine("Map (Слова з довжиною):");
            CollectionUtils.ForEach(wordLengths, w => Console.WriteLine($" - {w}"));

            string longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"MaxBy (Найдовше слово): {longestWord}");

            var longWords = CollectionUtils.Filter(words, w => w.Length > 5);
            Console.Write("Filter (Слова довше 5 символів): ");
            CollectionUtils.ForEach(longWords, w => Console.Write($"'{w}' "));
            Console.WriteLine("\n");

            // 3. Демонстрація з List<Product>
            Console.WriteLine("--- 3. ТЕСТУВАННЯ З ВХІДНОЮ КОЛЕКЦІЄЮ List<Product> ---");
            List<Product> products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 35000m },
                new Product { Name = "Mouse", Price = 750m },
                new Product { Name = "Keyboard", Price = 2200m },
                new Product { Name = "Monitor", Price = 9800m }
            };

            var productNames = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine($"Map (Список назв товарів): {string.Join(", ", productNames)}");

            Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"MaxBy (Найдорожчий продукт): {mostExpensive}");

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 2000m);
            Console.WriteLine("Filter (Товари дорожчі 2000 грн):");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" - {p}"));
        }
    }
}