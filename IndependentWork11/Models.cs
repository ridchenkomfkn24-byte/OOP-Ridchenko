using System;
using System.Collections.Generic;

namespace IndependentWork11
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

    public class SalesData
    {
        public List<Sale> Sales { get; } = new List<Sale>();
        public List<Product> Products { get; } = new List<Product>();
        public List<Customer> Customers { get; } = new List<Customer>();
    }

    public record ProductTotal(string Category, string Name, int Quantity);
    public record CityTotal(string City, decimal Revenue);
    public record MonthTotal(int Year, int Month, int Quantity, decimal Revenue);
    public record CustomerTotal(int Id, string Name, decimal Revenue);
}
