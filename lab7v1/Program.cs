using System;

namespace Lab7App
{
    public class Vehicle
    {
        public string Brand { get; set; }

        public Vehicle(string brand)
        {
            Brand = brand;
        }

        public virtual void Move()
        {
            Console.WriteLine($"[Vehicle ({Brand})] Рухається транспортний засіб.");
        }
    }

    public class Car : Vehicle
    {
        public int Speed { get; set; }

        public Car(string brand, int speed) : base(brand)
        {
            Speed = speed;
        }

        public override void Move()
        {
            Console.WriteLine($"[Car ({Brand})] Driving on the road зі швидкістю {Speed} км/год.");
        }
    }

    public class Bicycle : Vehicle
    {
        public bool HasBell { get; set; }

        public Bicycle(string brand, bool hasBell) : base(brand)
        {
            HasBell = hasBell;
        }

        public new void Move()
        {
            Console.WriteLine($"[Bicycle ({Brand})] Pedaling on the path. Дзвінок: {(HasBell ? "є" : "немає")}.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Виклик через посилання БАЗОВОГО класу (Vehicle) ===");
            Vehicle obj1 = new Car("Toyota", 120);
            Vehicle obj2 = new Bicycle("Trek", true);

            obj1.Move(); 
            obj2.Move(); 

            Console.WriteLine("\n=== 2. Виклик через посилання ПОХІДНИХ класів (Car / Bicycle) ===");
            ((Car)obj1).Move(); 
            ((Bicycle)obj2).Move(); 

            Console.WriteLine("\n=== 3. Порівняльний висновок у консолі ===");
            Console.WriteLine("override: забезпечує поліморфізм -> викликається реалізація об'єкта (Car).");
            Console.WriteLine("new: приховує метод -> викликається реалізація за типом посилання (Vehicle).");
        }
    }
}