using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace IndependentWork12
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    internal class Program
    {
        // Встановлюємо українську культуру для всієї програми
        private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = UkCulture;
            CultureInfo.CurrentUICulture = UkCulture;

            Console.WriteLine("=== Самостійна робота №12: LINQ pipelines: ETL та аналітика ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            var random = new Random(42);

            var categories = new[] { "Електроніка", "Одяг", "Товари для дому", "Книги", "Спорт" };
            var cities = new[] { "Київ", "Львів", "Одеса", "Харків", "Дніпро", "Рівне" };

            var products = Enumerable.Range(1, 60).Select(i => new Product
            {
                Id = i,
                Name = $"Товар_{i}",
                Category = categories[random.Next(categories.Length)],
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2)
            }).ToList();

            var customers = Enumerable.Range(1, 50).Select(i => new Customer
            {
                Id = i,
                Name = $"Клієнт_{i}",
                City = cities[random.Next(cities.Length)]
            }).ToList();

            var sales = Enumerable.Range(1, 100).Select(i => new Sale
            {
                Id = i,
                ProductId = random.Next(1, products.Count + 1),
                Quantity = random.Next(1, 15),
                SaleDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                CustomerId = random.Next(1, customers.Count + 1)
            }).ToList();

            Console.WriteLine($"Завантажено даних: Продукти: {products.Count}, Клієнти: {customers.Count}, Продажі: {sales.Count}\n");

            Pipeline1_TopProductsByCategory(sales, products);
            Pipeline2_RevenueByCustomerCity(sales, products, customers);
            Pipeline3_SalesByMonthAndYear(sales, products);
            Pipeline4_VIPCustomersAnalysis(sales, products, customers);
        }

        static void Pipeline1_TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            Console.WriteLine("--- Пайплайн 1: Найпопулярніші продукти за категоріями (Top 3) ---");
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { p.Name, p.Category, s.TotalQuantity })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TopProducts = g.OrderByDescending(x => x.TotalQuantity).Take(3).ToList()
                })
                .ToList();

            sw.Stop();

            foreach (var cat in result)
            {
                Console.WriteLine($"Категорія: {cat.Category}");
                foreach (var p in cat.TopProducts)
                {
                    Console.WriteLine($"  - {p.Name}: {p.TotalQuantity} шт.");
                }
            }
            Console.WriteLine($"Час виконання Пайплайну 1: {sw.Elapsed.TotalMilliseconds:F4} ms\n");
        }

        static void Pipeline2_RevenueByCustomerCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            Console.WriteLine("--- Пайплайн 2: Загальний дохід за містами клієнтів ---");
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, s.Quantity, p.UnitPrice })
                .Join(customers, x => x.CustomerId, c => c.Id, (x, c) => new { c.City, Revenue = x.Quantity * x.UnitPrice })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, TotalRevenue = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            foreach (var item in result)
            {
                Console.WriteLine($"Місто: {item.City,-8} | Дохід: {item.TotalRevenue:N2} грн");
            }
            Console.WriteLine($"Час виконання Пайплайну 2: {sw.Elapsed.TotalMilliseconds:F4} ms\n");
        }

        static void Pipeline3_SalesByMonthAndYear(List<Sale> sales, List<Product> products)
        {
            Console.WriteLine("--- Пайплайн 3: Продажі за місяцями та роками ---");
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            foreach (var item in result)
            {
                Console.WriteLine($"Рік: {item.Year}, Місяць: {item.Month:D2} | Одиниць: {item.TotalUnits,3} | Дохід: {item.TotalRevenue:N2} грн");
            }
            Console.WriteLine($"Час виконання Пайплайну 3: {sw.Elapsed.TotalMilliseconds:F4} ms\n");
        }

        static void Pipeline4_VIPCustomersAnalysis(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            Console.WriteLine("--- Пайплайн 4: Аналіз VIP-клієнтів (Чек > 1000 грн) ---");
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .Where(x => x.Revenue > 1000m)
                .Join(customers, x => x.CustomerId, c => c.Id, (x, c) => new { c.Name, c.City, x.Revenue })
                .GroupBy(x => new { x.Name, x.City })
                .Select(g => new
                {
                    CustomerName = g.Key.Name,
                    g.Key.City,
                    BigPurchasesCount = g.Count(),
                    TotalVIPSpend = g.Sum(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalVIPSpend)
                .Take(5)
                .ToList();

            sw.Stop();

            foreach (var c in result)
            {
                Console.WriteLine($"Клієнт: {c.CustomerName} ({c.City}) | Великих покупок: {c.BigPurchasesCount} | Сума: {c.TotalVIPSpend:N2} грн");
            }
            Console.WriteLine($"Час виконання Пайплайну 4: {sw.Elapsed.TotalMilliseconds:F4} ms\n");
        }
    }
}