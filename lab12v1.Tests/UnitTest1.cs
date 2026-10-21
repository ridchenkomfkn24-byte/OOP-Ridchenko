using System;
using System.Collections.Generic;
using System.Linq;
using lab12v1;
using Xunit;

namespace lab12v1.Tests
{
    public class InMemoryRepositoryTests
    {
        private InMemoryRepository<Product> CreateRepository()
        {
            return new InMemoryRepository<Product>();
        }

        [Fact]
        public void Add_ShouldAddProductToRepository()
        {
            var repo = CreateRepository();
            var product = new Product { Id = 1, Name = "Монітор", Price = 7500m, Stock = 3 };

            repo.Add(product);

            var result = repo.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("Монітор", result.Name);
        }

        [Fact]
        public void Add_DuplicateId_ShouldThrowInvalidOperationException()
        {
            var repo = CreateRepository();
            var p1 = new Product { Id = 1, Name = "Товар 1", Price = 100m, Stock = 1 };
            var p2 = new Product { Id = 1, Name = "Товар 2", Price = 200m, Stock = 2 };

            repo.Add(p1);

            Assert.Throws<InvalidOperationException>(() => repo.Add(p2));
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnCorrectProduct()
        {
            var repo = CreateRepository();
            repo.Add(new Product { Id = 5, Name = "Планшет", Price = 12000m, Stock = 2 });

            var result = repo.GetById(5);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldReturnNull()
        {
            var repo = CreateRepository();

            var result = repo.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            var repo = CreateRepository();
            repo.Add(new Product { Id = 1, Name = "A", Price = 10m, Stock = 1 });
            repo.Add(new Product { Id = 2, Name = "B", Price = 20m, Stock = 2 });

            var result = repo.GetAll();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Update_ExistingProduct_ShouldUpdateData()
        {
            var repo = CreateRepository();
            repo.Add(new Product { Id = 1, Name = "Стара назва", Price = 100m, Stock = 5 });

            var updatedProduct = new Product { Id = 1, Name = "Нова назва", Price = 150m, Stock = 10 };
            repo.Update(updatedProduct);

            var result = repo.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("Нова назва", result.Name);
            Assert.Equal(150m, result.Price);
            Assert.Equal(10, result.Stock);
        }

        [Fact]
        public void Update_NonExistingProduct_ShouldThrowKeyNotFoundException()
        {
            var repo = CreateRepository();
            var product = new Product { Id = 99, Name = "Фантом", Price = 50m, Stock = 1 };

            Assert.Throws<KeyNotFoundException>(() => repo.Update(product));
        }

        [Fact]
        public void Delete_ExistingProduct_ShouldRemoveItFromRepository()
        {
            var repo = CreateRepository();
            repo.Add(new Product { Id = 10, Name = "SSD Диск", Price = 3000m, Stock = 4 });

            repo.Delete(10);

            Assert.Null(repo.GetById(10));
            Assert.Empty(repo.GetAll());
        }

        [Fact]
        public void Delete_NonExistingProduct_ShouldThrowKeyNotFoundException()
        {
            var repo = CreateRepository();

            Assert.Throws<KeyNotFoundException>(() => repo.Delete(999));
        }
    }
}