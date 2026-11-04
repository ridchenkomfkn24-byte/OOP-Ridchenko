using System;
using System.Collections.Generic;

namespace IndependentWork8
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public Cache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Ємність кешу повинна бути більше нуля.", nameof(capacity));
            }

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>();
            _keysOrder = new Queue<TKey>();
        }

        public int Count => _cache.Count;

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено у кеші.");
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _cache.ContainsKey(key);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Самостійна робота №8: Узагальнений кеш Cache<TKey, TValue> ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            var cache = new Cache<string, string>(3);

            Console.WriteLine("--- Додавання 3 елементів (ємність = 3) ---");
            cache.Add("k1", "Значення 1");
            cache.Add("k2", "Значення 2");
            cache.Add("k3", "Значення 3");
            Console.WriteLine($"Поточна кількість елементів (Count): {cache.Count}");
            Console.WriteLine($"Отримання 'k1': {cache.Get("k1")}");

            Console.WriteLine("\n--- Додавання 4-го елемента 'k4' (витіснення найстарішого 'k1' за FIFO) ---");
            cache.Add("k4", "Значення 4");
            Console.WriteLine($"Чи містить кеш 'k1'?: {cache.ContainsKey("k1")}");
            Console.WriteLine($"Чи містить кеш 'k4'?: {cache.ContainsKey("k4")}");
            Console.WriteLine($"Поточна кількість елементів (Count): {cache.Count}");

            try
            {
                Console.WriteLine("\n--- Спроба отримати витіснений ключ 'k1' ---");
                cache.Get("k1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Спіймано виняток: {ex.Message}");
            }
        }
    }
}