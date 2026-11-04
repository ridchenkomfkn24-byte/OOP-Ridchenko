using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork11
{
    public static class SalesAnalytics
    {
        public static List<ProductTotal> TopProductsByCategory(SalesData data)
        {
            return data.Sales
                .GroupBy(sale => sale.ProductId)
                .Select(group => new { ProductId = group.Key, Quantity = group.Sum(sale => sale.Quantity) })
                .Join(data.Products, total => total.ProductId, product => product.Id,
                    (total, product) => new ProductTotal(product.Category, product.Name, total.Quantity))
                .GroupBy(total => total.Category)
                .OrderBy(group => group.Key)
                .SelectMany(group => group.OrderByDescending(total => total.Quantity)
                    .ThenBy(total => total.Name).Take(3))
                .ToList();
        }

        public static List<CityTotal> RevenueByCity(SalesData data)
        {
            return data.Sales
                .Join(data.Products, sale => sale.ProductId, product => product.Id,
                    (sale, product) => new { sale.CustomerId, Revenue = sale.Quantity * product.UnitPrice })
                .Join(data.Customers, sale => sale.CustomerId, customer => customer.Id,
                    (sale, customer) => new { customer.City, sale.Revenue })
                .GroupBy(row => row.City)
                .Select(group => new CityTotal(group.Key, group.Sum(row => row.Revenue)))
                .OrderByDescending(total => total.Revenue).ThenBy(total => total.City)
                .ToList();
        }

        public static List<MonthTotal> MonthlySales(SalesData data)
        {
            return data.Sales
                .Join(data.Products, sale => sale.ProductId, product => product.Id,
                    (sale, product) => new { sale.SaleDate, sale.Quantity, Revenue = sale.Quantity * product.UnitPrice })
                .GroupBy(row => new { row.SaleDate.Year, row.SaleDate.Month })
                .Select(group => new MonthTotal(group.Key.Year, group.Key.Month,
                    group.Sum(row => row.Quantity), group.Sum(row => row.Revenue)))
                .OrderBy(total => total.Year).ThenBy(total => total.Month)
                .ToList();
        }

        public static List<CustomerTotal> TopCustomers(SalesData data, DateTime fromDate)
        {
            return data.Sales
                .Where(sale => sale.SaleDate >= fromDate)
                .Join(data.Products, sale => sale.ProductId, product => product.Id,
                    (sale, product) => new { sale.CustomerId, Revenue = sale.Quantity * product.UnitPrice })
                .GroupBy(row => row.CustomerId)
                .Select(group => new { CustomerId = group.Key, Revenue = group.Sum(row => row.Revenue) })
                .Join(data.Customers, total => total.CustomerId, customer => customer.Id,
                    (total, customer) => new CustomerTotal(customer.Id, customer.Name, total.Revenue))
                .OrderByDescending(total => total.Revenue).ThenBy(total => total.Id)
                .Take(5).ToList();
        }
    }
}
