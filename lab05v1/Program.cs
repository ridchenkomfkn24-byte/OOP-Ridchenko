using System;

namespace Lab5App
{
    public class IntArray
    {
        private int[] _data;

        public int Length => _data.Length;

        public IntArray(int size)
        {
            _data = new int[size];
        }

        public IntArray(params int[] items)
        {
            _data = items != null ? (int[])items.Clone() : new int[0];
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= _data.Length)
                    throw new IndexOutOfRangeException($"Індекс {index} виходить за межі масиву.");
                return _data[index];
            }
            set
            {
                if (index < 0 || index >= _data.Length)
                    throw new IndexOutOfRangeException($"Індекс {index} виходить за межі масиву.");
                _data[index] = value;
            }
        }

        public static IntArray operator +(IntArray a, IntArray b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException("Операнди не можуть бути null.");

            int maxLen = Math.Max(a.Length, b.Length);
            IntArray result = new IntArray(maxLen);

            for (int i = 0; i < maxLen; i++)
            {
                int valA = i < a.Length ? a[i] : 0;
                int valB = i < b.Length ? b[i] : 0;
                result[i] = valA + valB;
            }

            return result;
        }

        public static bool operator ==(IntArray? a, IntArray? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(IntArray? a, IntArray? b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is IntArray other)
            {
                if (_data.Length != other._data.Length) return false;
                for (int i = 0; i < _data.Length; i++)
                {
                    if (_data[i] != other._data[i]) return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var item in _data)
            {
                hash = hash * 31 + item.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _data)}]";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IntArray arr1 = new IntArray(10, 20, 30);
            IntArray arr2 = new IntArray(5, 15, 25);

            Console.WriteLine($"arr1: {arr1}");
            Console.WriteLine($"arr2: {arr2}");

            Console.WriteLine($"\narr1[1] до зміни: {arr1[1]}");
            arr1[1] = 99;
            Console.WriteLine($"arr1[1] після зміни: {arr1[1]}");
            Console.WriteLine($"Оновлений arr1: {arr1}");

            IntArray sumArr = arr1 + arr2;
            Console.WriteLine($"\narr1 + arr2 = {sumArr}");

            IntArray arr3 = new IntArray(10, 99, 30);

            Console.WriteLine($"\narr1: {arr1}");
            Console.WriteLine($"arr3: {arr3}");
            Console.WriteLine($"arr1 == arr3: {arr1 == arr3}");
            Console.WriteLine($"arr1 != arr2: {arr1 != arr2}");
        }
    }
}