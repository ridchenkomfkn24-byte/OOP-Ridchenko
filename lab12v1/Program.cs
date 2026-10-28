using System;
using System.Collections.Generic;
using System.Linq;

namespace lab12v1
{
    // Інтерфейс базової сутності з ID
    public interface IEntity
    {
        int Id { get; set; }
    }

    // Сутність продукту (Варіант 1)
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public override string ToString()
        {
            return $"[ID: {Id}] {Name} | Ціна: {Price:C} | Кількість: {Stock} шт.";
        }
    }

    // Узагальнений інтерфейс репозиторію з where-обмеженням
    public interface IRepository<T> where T : class, IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    // Узагальнена реалізація In-Memory репозиторію
    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_items.Any(i => i.Id == item.Id))
            {
                throw new InvalidOperationException($"Елемент із ID {item.Id} вже існує.");
            }
            _items.Add(item);
        }

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public void Update(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            var existingIndex = _items.FindIndex(i => i.Id == item.Id);
            if (existingIndex == -1)
            {
                throw new KeyNotFoundException($"Елемент із ID {item.Id} не знайдено для оновлення.");
            }

            _items[existingIndex] = item;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Елемент із ID {id} не знайдено для видалення.");
            }
            _items.Remove(item);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №12: Узагальнені типи (Generics) та xUnit Тести ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            IRepository<Product> repository = new InMemoryRepository<Product>();

            Console.WriteLine("--- 1. Додавання товарів ---");
            repository.Add(new Product { Id = 1, Name = "Ноутбук Lenovo", Price = 28500m, Stock = 5 });
            repository.Add(new Product { Id = 2, Name = "Миша Logitech", Price = 1200m, Stock = 15 });
            repository.Add(new Product { Id = 3, Name = "Клавіатура Keychron", Price = 4300m, Stock = 8 });

            foreach (var product in repository.GetAll())
            {
                Console.WriteLine(product);
            }

            Console.WriteLine("\n--- 2. Оновлення товару ID 2 ---");
            var mouse = repository.GetById(2);
            if (mouse != null)
            {
                mouse.Price = 1050m;
                mouse.Stock = 20;
                repository.Update(mouse);
            }
            Console.WriteLine(repository.GetById(2));

            Console.WriteLine("\n--- 3. Видалення товару ID 1 ---");
            repository.Delete(1);

            Console.WriteLine("\nЗалишок у репозиторії:");
            foreach (var product in repository.GetAll())
            {
                Console.WriteLine(product);
            }
        }
    }
}