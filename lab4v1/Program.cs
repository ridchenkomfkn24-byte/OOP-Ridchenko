using System;

namespace OOPLab4
{
    /// <summary>
    /// Представляє точку у двовимірному просторі (2D)
    /// </summary>
    public class Point
    {
        // 1. Приватні поля
        private int _x;
        private int _y;

        // Константи для валідації
        public const int MinValue = -1000;
        public const int MaxValue = 1000;

        // 2. Публічні властивості з валідацією
        public int X
        {
            get => _x;
            set
            {
                if (value < MinValue || value > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value), 
                        $"Координата X повинна бути в діапазоні від {MinValue} до {MaxValue}."
                    );
                }
                _x = value;
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (value < MinValue || value > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value), 
                        $"Координата Y повинна бути в діапазоні від {MinValue} до {MaxValue}."
                    );
                }
                _y = value;
            }
        }

        // 3. Статичний член (повертає точку початку координат (0,0))
        public static Point Origin => new Point(0, 0);

        // Конструктор за замовчуванням
        public Point() : this(0, 0) { }

        // Конструктор із параметрами
        public Point(int x, int y)
        {
            X = x; // Використовуємо властивості для автоматичної перевірки
            Y = y;
        }

        // 4. Індексатор (0 - X, 1 - Y)
        public int this[int index]
        {
            get
            {
                if (index == 0) return X;
                if (index == 1) return Y;
                throw new IndexOutOfRangeException("Індекс повинен бути 0 (X) або 1 (Y).");
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else throw new IndexOutOfRangeException("Індекс повинен бути 0 (X) або 1 (Y).");
            }
        }

        // 5. Перевантаження операторів

        // Додавання двох точок: p1 + p2
        public static Point operator +(Point p1, Point p2)
        {
            if (p1 is null || p2 is null)
                throw new ArgumentNullException("Операнди не можуть бути null.");

            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        // Множення точки на скаляр: p * scalar
        public static Point operator *(Point p, int scalar)
        {
            if (p is null)
                throw new ArgumentNullException(nameof(p));

            return new Point(p.X * scalar, p.Y * scalar);
        }

        // Множення скаляра на точку: scalar * p
        public static Point operator *(int scalar, Point p)
        {
            return p * scalar;
        }

        // Оператор рівності ==
        public static bool operator ==(Point? p1, Point? p2)
        {
            if (ReferenceEquals(p1, p2)) return true;
            if (p1 is null || p2 is null) return false;

            return p1.Equals(p2);
        }

        // Оператор нерівності !=
        public static bool operator !=(Point? p1, Point? p2)
        {
            return !(p1 == p2);
        }

        // 6. Перевизначення стандартних методів System.Object

        public override bool Equals(object? obj)
        {
            if (obj is Point otherPoint)
            {
                return X == otherPoint.X && Y == otherPoint.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"Point({X}, {Y})";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №4: Властивості, Статичні члени, Індексатори та Оператори ===");
            Console.WriteLine();

            // 1. Створення об'єктів та демонстрація статичного члена
            Console.WriteLine("1. Створення об'єктів та використання статичного члена:");
            Point p1 = new Point(10, 20);
            Point p2 = new Point(5, -15);
            Point origin = Point.Origin; // Статична властивість

            Console.WriteLine($"Точка p1: {p1}");
            Console.WriteLine($"Точка p2: {p2}");
            Console.WriteLine($"Початок координат (Point.Origin): {origin}");
            Console.WriteLine();

            // 2. Демонстрація індексатора
            Console.WriteLine("2. Демонстрація індексатора [0] та [1]:");
            Console.WriteLine($"p1[0] (координата X): {p1[0]}");
            Console.WriteLine($"p1[1] (координата Y): {p1[1]}");

            p1[0] = 15; // Зміна X через індексатор
            p1[1] = 25; // Зміна Y через індексатор
            Console.WriteLine($"Змінена p1 через індексатори: {p1}");
            Console.WriteLine();

            // 3. Демонстрація валідації властивостей
            Console.WriteLine("3. Демонстрація валідації (діапазон від -1000 до 1000):");
            try
            {
                Console.WriteLine("Спроба встановити X = 1500...");
                p1.X = 1500;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[ПОМИЛКА СПІЙМАНА]: {ex.Message}");
            }
            Console.WriteLine();

            // 4. Демонстрація перевантаження операторів +, *
            Console.WriteLine("4. Перевантажені арифметичні оператори:");
            Point sum = p1 + p2;
            Console.WriteLine($"Додавання (p1 + p2): {p1} + {p2} = {sum}");

            Point multiplied = p1 * 3;
            Console.WriteLine($"Множення на скаляр (p1 * 3): {p1} * 3 = {multiplied}");
            Console.WriteLine();

            // 5. Демонстрація операторів порівняння та методів Equals / GetHashCode
            Console.WriteLine("5. Порівняння об'єктів (==, !=, Equals, GetHashCode):");
            Point p3 = new Point(15, 25); // Однакові координати з p1

            Console.WriteLine($"p1: {p1}");
            Console.WriteLine($"p3: {p3}");
            Console.WriteLine($"p1 == p3: {p1 == p3}");
            Console.WriteLine($"p1 != p2: {p1 != p2}");
            Console.WriteLine($"p1.Equals(p3): {p1.Equals(p3)}");
            Console.WriteLine($"Hash code p1: {p1.GetHashCode()}");
            Console.WriteLine($"Hash code p3: {p3.GetHashCode()}");
        }
    }
}