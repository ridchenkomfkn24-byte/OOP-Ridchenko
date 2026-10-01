using System;

namespace IndependentWork3
{
    public class UserConfiguration
    {
        private readonly string _appName;
        private int _maxUsers;

        public string AppName => _appName;

        public int MaxUsers
        {
            get => _maxUsers;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Кількість користувачів повинна бути більшою за 0.");
                }
                _maxUsers = value;
            }
        }

        public UserConfiguration(string appName, int maxUsers)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentNullException(nameof(appName), "Назва додатка не може бути порожньою.");
            }

            _appName = appName;
            MaxUsers = maxUsers; // Використовує валідацію сеттера
        }

        public override string ToString()
        {
            return $"App: {AppName}, MaxUsers: {MaxUsers}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Самостійна робота №3: Демонстрація інкапсуляції та валідації ===\n");

            try
            {
                UserConfiguration config = new UserConfiguration("AutoMapperDemo", 100);
                Console.WriteLine($"Успішно створено конфігурацію: {config}");

                Console.WriteLine("\nСпроба встановити некоректне значення MaxUsers (-5)...");
                config.MaxUsers = -5;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА ВАЛІДАЦІЇ]: {ex.Message}");
            }
        }
    }
}