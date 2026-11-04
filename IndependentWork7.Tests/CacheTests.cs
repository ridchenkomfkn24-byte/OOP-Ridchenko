using System;
using System.Collections.Generic;
using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_ProductPrices_ShouldStoreAndUpdateDecimalValues()
        {
            var cache = new Cache<string, decimal>(2);
            cache.Add("Ноутбук", 45000.00m);
            cache.Add("Смартфон", 22500.50m);

            cache.Add("Смартфон", 21000.00m);

            Assert.Equal(21000.00m, cache.Get("Смартфон"));
            Assert.Equal(2, cache.Count);

            cache.Add("Монітор", 11800.00m);

            Assert.False(cache.ContainsKey("Ноутбук"));
            Assert.Equal(21000.00m, cache.Get("Смартфон"));
            Assert.Equal(11800.00m, cache.Get("Монітор"));
        }

        [Fact]
        public void Add_ShouldStoreElements()
        {
            var cache = new Cache<string, string>(3);

            cache.Add("A", "Перший");
            cache.Add("B", "Другий");

            Assert.Equal("Перший", cache.Get("A"));
            Assert.Equal("Другий", cache.Get("B"));
        }

        [Fact]
        public void Get_ExistingKey_ShouldReturnValue()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Значення");

            Assert.Equal("Значення", cache.Get(1));
        }

        [Fact]
        public void Get_MissingKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<int, string>(2);

            Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
        }

        [Fact]
        public void Add_WhenFull_ShouldRemoveOldestElement()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Перший");
            cache.Add(2, "Другий");

            cache.Add(3, "Третій");

            Assert.False(cache.ContainsKey(1));
            Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
            Assert.Equal("Другий", cache.Get(2));
            Assert.Equal("Третій", cache.Get(3));
        }

        [Fact]
        public void Get_ShouldNotChangeFifoOrder()
        {
            var cache = new Cache<int, int>(2);
            cache.Add(1, 10);
            cache.Add(2, 20);

            cache.Get(1);
            cache.Add(3, 30);

            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
        }

        [Fact]
        public void ContainsKey_ExistingKey_ShouldReturnTrue()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Значення");

            Assert.True(cache.ContainsKey(1));
        }

        [Fact]
        public void ContainsKey_MissingKey_ShouldReturnFalse()
        {
            var cache = new Cache<int, string>(2);

            Assert.False(cache.ContainsKey(99));
        }

        [Fact]
        public void Count_NewCache_ShouldBeZero()
        {
            var cache = new Cache<int, int>(2);

            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void Count_ShouldIncreaseUntilCapacityAndRemainBounded()
        {
            var cache = new Cache<int, int>(2);
            cache.Add(1, 10);
            Assert.Equal(1, cache.Count);
            cache.Add(2, 20);
            Assert.Equal(2, cache.Count);

            cache.Add(3, 30);

            Assert.Equal(2, cache.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_InvalidCapacity_ShouldThrowArgumentOutOfRangeException(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(capacity));
        }

        [Fact]
        public void Add_CapacityOne_ShouldReplaceOldestElement()
        {
            var cache = new Cache<int, int>(1);
            cache.Add(1, 10);

            cache.Add(2, 20);

            Assert.False(cache.ContainsKey(1));
            Assert.Equal(20, cache.Get(2));
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateWithoutChangingCountOrOrder()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "Старе значення");
            cache.Add(2, "Другий");

            cache.Add(1, "Нове значення");

            Assert.Equal("Нове значення", cache.Get(1));
            Assert.Equal(2, cache.Count);
            Assert.Equal("Другий", cache.Get(2));

            cache.Add(3, "Третій");
            Assert.False(cache.ContainsKey(1));
            cache.Add(4, "Четвертий");
            Assert.False(cache.ContainsKey(2));
            Assert.Equal("Третій", cache.Get(3));
            Assert.Equal("Четвертий", cache.Get(4));
        }

        [Fact]
        public void Add_RepeatedOverflow_ShouldKeepNewestElements()
        {
            var cache = new Cache<int, int>(3);

            for (int key = 1; key <= 10; key++)
            {
                cache.Add(key, key * 10);
            }

            Assert.Equal(3, cache.Count);
            for (int key = 1; key <= 7; key++)
            {
                Assert.False(cache.ContainsKey(key));
            }
            Assert.Equal(80, cache.Get(8));
            Assert.Equal(90, cache.Get(9));
            Assert.Equal(100, cache.Get(10));
        }
    }
}
