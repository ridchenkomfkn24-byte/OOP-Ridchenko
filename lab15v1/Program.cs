using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab15v1
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №15: LINQ в дії ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" },
                new Category { Id = 3, Name = "Clothing" },
                new Category { Id = 4, Name = "Home & Kitchen" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop Pro", Price = 1200m, CategoryId = 1 },
                new Product { Id = 2, Name = "Smartphone X", Price = 800m, CategoryId = 1 },
                new Product { Id = 3, Name = "Wireless Mouse", Price = 25m, CategoryId = 1 },
                new Product { Id = 4, Name = "4K Monitor", Price = 600m, CategoryId = 1 },
                new Product { Id = 5, Name = "USB Cable", Price = 15m, CategoryId = 1 },
                new Product { Id = 6, Name = "C# Programming Book", Price = 45m, CategoryId = 2 },
                new Product { Id = 7, Name = "Algorithms & Data Structures", Price = 60m, CategoryId = 2 },
                new Product { Id = 8, Name = "Fiction Novel", Price = 20m, CategoryId = 2 },
                new Product { Id = 9, Name = "Winter Jacket", Price = 150m, CategoryId = 3 },
                new Product { Id = 10, Name = "T-Shirt", Price = 25m, CategoryId = 3 },
                new Product { Id = 11, Name = "Jeans", Price = 70m, CategoryId = 3 },
                new Product { Id = 12, Name = "Coffee Maker", Price = 110m, CategoryId = 4 },
                new Product { Id = 13, Name = "Microwave Oven", Price = 220m, CategoryId = 4 },
                new Product { Id = 14, Name = "Blender", Price = 85m, CategoryId = 4 },
                new Product { Id = 15, Name = "Smart TV", Price = 950m, CategoryId = 1 }
            };

            Stopwatch sw = new Stopwatch();

            // ЗАПИТ 1: Продукти в категорії "Electronics" з ціною > 500
            Console.WriteLine("=== Запит 1: Продукти в 'Electronics' з ціною > 500 ===");
            
            // Syntax 1: Query Syntax
            sw.Restart();
            var query1_1 = (from p in products
                            join c in categories on p.CategoryId equals c.Id
                            where c.Name == "Electronics" && p.Price > 500
                            select p).ToList();
            sw.Stop();
            Console.WriteLine($"[Query Syntax] Знайдено: {query1_1.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");

            // Syntax 2: Method Syntax
            sw.Restart();
            var query1_2 = products
                .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { Product = p, Category = c })
                .Where(x => x.Category.Name == "Electronics" && x.Product.Price > 500)
                .Select(x => x.Product)
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Method Syntax] Знайдено: {query1_2.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");
            foreach (var p in query1_2) Console.WriteLine($" - {p.Name}: {p.Price:C}");

            // ЗАПИТ 2: Об'єднання продуктів з їхніми категоріями
            Console.WriteLine("\n=== Запит 2: Об'єднання продуктів з категоріями (Join) ===");

            // Syntax 1: Query Syntax
            sw.Restart();
            var query2_1 = (from p in products
                            join c in categories on p.CategoryId equals c.Id
                            select new { ProductName = p.Name, CategoryName = c.Name }).ToList();
            sw.Stop();
            Console.WriteLine($"[Query Syntax] Елементів: {query2_1.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");

            // Syntax 2: Method Syntax
            sw.Restart();
            var query2_2 = products
                .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { ProductName = p.Name, CategoryName = c.Name })
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Method Syntax] Елементів: {query2_2.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");
            foreach (var item in query2_2.Take(5)) Console.WriteLine($" - {item.ProductName} ({item.CategoryName})");

            // ЗАПИТ 3: Середня ціна продуктів у кожній категорії
            Console.WriteLine("\n=== Запит 3: Середня ціна у кожній категорії ===");

            // Syntax 1: Query Syntax
            sw.Restart();
            var query3_1 = (from p in products
                            join c in categories on p.CategoryId equals c.Id
                            group p by c.Name into g
                            select new { CategoryName = g.Key, AvgPrice = g.Average(p => p.Price) }).ToList();
            sw.Stop();
            Console.WriteLine($"[Query Syntax] Категорій: {query3_1.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");

            // Syntax 2: Method Syntax
            sw.Restart();
            var query3_2 = products
                .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { p.Price, CategoryName = c.Name })
                .GroupBy(x => x.CategoryName)
                .Select(g => new { CategoryName = g.Key, AvgPrice = g.Average(x => x.Price) })
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Method Syntax] Категорій: {query3_2.Count} (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");
            foreach (var item in query3_2) Console.WriteLine($" - {item.CategoryName}: {item.AvgPrice:C}");

            // ЗАПИТ 4: Найдорожчий продукт
            Console.WriteLine("\n=== Запит 4: Найдорожчий продукт ===");

            // Syntax 1: Query Syntax
            sw.Restart();
            var query4_1 = (from p in products
                            orderby p.Price descending
                            select p).FirstOrDefault();
            sw.Stop();
            Console.WriteLine($"[Query Syntax] Найдорожчий: {query4_1?.Name} ({query4_1?.Price:C}) (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");

            // Syntax 2: Method Syntax
            sw.Restart();
            var query4_2 = products.OrderByDescending(p => p.Price).FirstOrDefault();
            sw.Stop();
            Console.WriteLine($"[Method Syntax] Найдорожчий: {query4_2?.Name} ({query4_2?.Price:C}) (Час: {sw.Elapsed.TotalMilliseconds:F4} ms)");
        }
    }
}