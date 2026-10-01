using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lab8App
{
    public class Shape
    {
        public string Color { get; set; }

        public Shape(string color)
        {
            Color = color;
        }

        public virtual double GetArea()
        {
            return 0.0;
        }

        public virtual double GetPerimeter()
        {
            return 0.0;
        }
    }

    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(string color, double radius) : base(color)
        {
            Radius = radius;
        }

        public override double GetArea()
        {
            return Math.PI * Radius * Radius;
        }

        public override double GetPerimeter()
        {
            return 2 * Math.PI * Radius;
        }
    }

    public class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(string color, double width, double height) : base(color)
        {
            Width = width;
            Height = height;
        }

        public override double GetArea()
        {
            return Width * Height;
        }

        public override double GetPerimeter()
        {
            return 2 * (Width + Height);
        }
    }

    public class Triangle : Shape
    {
        public double SideA { get; set; }
        public double SideB { get; set; }
        public double SideC { get; set; }

        public Triangle(string color, double sideA, double sideB, double sideC) : base(color)
        {
            SideA = sideA;
            SideB = sideB;
            SideC = sideC;
        }

        public override double GetArea()
        {
            double p = GetPerimeter() / 2.0;
            return Math.Sqrt(p * (p - SideA) * (p - SideB) * (p - SideC));
        }

        public override double GetPerimeter()
        {
            return SideA + SideB + SideC;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            List<Shape> shapes = new List<Shape>
            {
                new Circle("Червоний", 5.0),
                new Rectangle("Синій", 4.0, 6.0),
                new Triangle("Зелений", 3.0, 4.0, 5.0)
            };

            Console.WriteLine("=== Перелік фігур та їх характеристики ===");
            foreach (var shape in shapes)
            {
                Console.WriteLine($"Фігура: {shape.GetType().Name} ({shape.Color}) | Площа: {shape.GetArea():F2} | Периметр: {shape.GetPerimeter():F2}");
            }

            double totalArea = shapes.Sum(s => s.GetArea());
            double totalPerimeter = shapes.Sum(s => s.GetPerimeter());

            Console.WriteLine("\n=== Агреговані результати ===");
            Console.WriteLine($"Загальна площа всіх фігур: {totalArea:F2}");
            Console.WriteLine($"Загальний периметр всіх фігур: {totalPerimeter:F2}");
        }
    }
}