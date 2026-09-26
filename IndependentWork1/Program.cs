using System;

namespace IndependentWork1
{
    public class Rectangle
    {
        private double _width;
        private double _height;

        public double Width
        {
            get { return _width; }
            set 
            { 
                if (value > 0) _width = value; 
            }
        }

        public double Height
        {
            get { return _height; }
        }

        public Rectangle(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public double CalculateArea()
        {
            return _width * _height;
        }

        public double CalculatePerimeter()
        {
            return 2 * (_width + _height);
        }
    }

    public class Employee
    {
        private string _fullName;
        private double _monthlySalary;

        public string FullName
        {
            get { return _fullName; }
            private set { _fullName = value; }
        }

        public double MonthlySalary
        {
            get { return _monthlySalary; }
            set 
            { 
                if (value >= 0) _monthlySalary = value; 
            }
        }

        public Employee(string fullName, double monthlySalary)
        {
            _fullName = fullName;
            _monthlySalary = monthlySalary;
        }

        public double CalculateAnnualIncome(double bonusPercentage)
        {
            double baseAnnual = _monthlySalary * 12;
            double bonus = baseAnnual * (bonusPercentage / 100);
            return baseAnnual + bonus;
        }
    }

    public class Playlist
    {
        private string _title;
        private int _trackCount;
        private int _totalDurationInSeconds;

        public string Title
        {
            get { return _title; }
        }

        public int TrackCount
        {
            get { return _trackCount; }
        }

        public Playlist(string title)
        {
            _title = title;
            _trackCount = 0;
            _totalDurationInSeconds = 0;
        }

        public void AddTrack(int durationInSeconds)
        {
            if (durationInSeconds > 0)
            {
                _trackCount++;
                _totalDurationInSeconds += durationInSeconds;
            }
        }

        public double GetTotalDurationInMinutes()
        {
            return _totalDurationInSeconds / 60.0;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСІВ ===\n");

            Console.WriteLine("--- 1. Клас Rectangle ---");
            Rectangle rect = new Rectangle(5.5, 10.0);
            Console.WriteLine($"Прямокутник: Ширина = {rect.Width}, Висота = {rect.Height}");
            Console.WriteLine($"Площа: {rect.CalculateArea()}");
            Console.WriteLine($"Периметр: {rect.CalculatePerimeter()}\n");

            Console.WriteLine("--- 2. Клас Employee ---");
            Employee emp = new Employee("Олексій Коваленко", 25000.00);
            double bonusPercent = 15.0;
            double annualIncome = emp.CalculateAnnualIncome(bonusPercent);
            Console.WriteLine($"Працівник: {emp.FullName}");
            Console.WriteLine($"Місячний оклад: {emp.MonthlySalary:N2} грн");
            Console.WriteLine($"Річний дохід (з бонусом {bonusPercent}%): {annualIncome:N2} грн\n");

            Console.WriteLine("--- 3. Клас Playlist ---");
            Playlist myPlaylist = new Playlist("Workout Hits");
            myPlaylist.AddTrack(210);
            myPlaylist.AddTrack(185);
            myPlaylist.AddTrack(240);

            Console.WriteLine($"Плейлист: \"{myPlaylist.Title}\"");
            Console.WriteLine($"Кількість треків: {myPlaylist.TrackCount}");
            Console.WriteLine($"Загальна тривалість: {myPlaylist.GetTotalDurationInMinutes():F2} хв.");
        }
    }
}