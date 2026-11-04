using System;
using System.Collections.Generic;
using IndependentWork8;
using Xunit;

namespace IndependentWork8.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldStoreAndReturnCorrectValue()
        {
            var cache = new Cache<int, string>(3);

            cache.Add(1, "One");
            cache.Add(2, "Two");

            Assert.Equal("One", cache.Get(1));
            Assert.Equal("Two", cache.Get(2));
        }

        [Fact]
        public void Count_ShouldReturnCorrectNumberOfElements()
        {
            var cache = new Cache<string, int>(5);

            cache.Add("A", 100);
            cache.Add("B", 200);

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueForExistingKeyAndFalseForMissing()
        {
            var cache = new Cache<string, string>(3);
            cache.Add("User", "Admin");

            Assert.True(cache.ContainsKey("User"));
            Assert.False(cache.ContainsKey("Guest"));
        }

        [Fact]
        public void Get_ShouldThrowKeyNotFoundException_WhenKeyDoesNotExist()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("Key1", "Val1");

            Assert.Throws<KeyNotFoundException>(() => cache.Get("NonExistingKey"));
        }

        [Fact]
        public void Add_ShouldEvictOldestElement_WhenCapacityIsExceeded_FIFO()
        {
            var cache = new Cache<string, string>(2);

            cache.Add("First", "1");
            cache.Add("Second", "2");
            cache.Add("Third", "3");

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey("First"));
            Assert.True(cache.ContainsKey("Second"));
            Assert.True(cache.ContainsKey("Third"));
        }

        [Fact]
        public void Add_ShouldUpdateValue_WhenKeyAlreadyExists()
        {
            var cache = new Cache<string, string>(2);

            cache.Add("Key", "OldValue");
            cache.Add("Key", "NewValue");

            Assert.Equal(1, cache.Count);
            Assert.Equal("NewValue", cache.Get("Key"));
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenCapacityIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => new Cache<int, int>(0));
            Assert.Throws<ArgumentException>(() => new Cache<int, int>(-5));
        }
    }
}