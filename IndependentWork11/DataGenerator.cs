using System;

namespace IndependentWork11
{
    public static class DataGenerator
    {
        public static SalesData Create()
        {
            var random = new Random(42);
            var data = new SalesData();
            string[] categories = { "Електроніка", "Книги", "Одяг", "Спорт" };
            string[] cities = { "Рівне", "Київ", "Львів", "Луцьк", "Тернопіль" };
            for (int id = 1; id <= 100; id++)
            {
                data.Products.Add(new Product
                {
                    Id = id,
                    Name = $"Товар {id}",
                    Category = categories[(id - 1) % categories.Length],
                    UnitPrice = random.Next(100, 10001)
                });
                data.Customers.Add(new Customer
                {
                    Id = id,
                    Name = $"Клієнт {id}",
                    City = cities[(id - 1) % cities.Length]
                });
            }

            for (int id = 1; id <= 1000; id++)
            {
                data.Sales.Add(new Sale
                {
                    Id = id,
                    ProductId = random.Next(1, 101),
                    CustomerId = random.Next(1, 101),
                    Quantity = random.Next(1, 6),
                    SaleDate = new DateTime(2025, 1, 1).AddDays(random.Next(0, 730))
                });
            }
            return data;
        }
    }
}
