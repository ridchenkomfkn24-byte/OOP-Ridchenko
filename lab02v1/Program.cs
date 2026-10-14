using System;

namespace Lab2
{
    // Варіант 1: Клас Book
    public class Book
    {
        // 1. Приватні поля
        private string _title;
        private string _author;
        private int _year;

        // 2. Публічні властивості
        public string Title
        {
            get => _title;
            set => _title = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Author
        {
            get => _author;
            set => _author = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public int Year
        {
            get => _year;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value > currentYear)
                {
                    Console.WriteLine($"[Валідація] Помилка: Рік видання ({value}) не може бути в майбутньому. Встановлено поточний рік ({currentYear}).");
                    _year = currentYear;
                }
                else if (value < 0)
                {
                    Console.WriteLine($"[Валідація] Помилка: Рік не може бути від'ємним. Встановлено 0.");
                    _year = 0;
                }
                else
                {
                    _year = value;
                }
            }
        }

        // 3. Конструктори

        // Головний параметризований конструктор
        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
            Console.WriteLine($"[Конструктор] Створено об'єкт книги: \"{Title}\"");
        }

        // Конструктор із 2 параметрами (викликає головний через : this())
        public Book(string title, string author) 
            : this(title, author, DateTime.Now.Year)
        {
        }

        // Конструктор за замовчуванням (викликає параметризований через : this())
        public Book() 
            : this("Unknown", "Unknown", DateTime.Now.Year)
        {
        }

        // 4. Метод класу
        public string GetFullInfo()
        {
            return $"Книга: \"{Title}\" | Автор: {Author} | Рік видання: {Year}";
        }

        // 5. Деструктор (Фіналізатор)
        ~Book()
        {
            Console.WriteLine($"[Деструктор] Збирач сміття знищив об'єкт книги: \"{Title}\"");
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Створення об'єктів ===");

            // 1. Використання конструктора за замовчуванням
            Book book1 = new Book();
            Console.WriteLine(book1.GetFullInfo());
            Console.WriteLine();

            // 2. Використання конструктора з 2 параметрами
            Book book2 = new Book("Кобзар", "Тарас Шевченко");
            Console.WriteLine(book2.GetFullInfo());
            Console.WriteLine();

            // 3. Використання повноцінного параметризованого конструктора (із завідомо некоректним роком для перевірки валідації)
            Book book3 = new Book("Майбутнє ООП", "Іван Іванов", 2099);
            Console.WriteLine(book3.GetFullInfo());
            Console.WriteLine();

            Console.WriteLine("=== Завершення роботи Main, підготовка до GC ===");

            // Очищаємо посилання на об'єкти, щоб GC міг їх зібрати
            book1 = null;
            book2 = null;
            book3 = null;

            // Примусовий виклик збирача сміття (виключно для демонстрації)
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("=== Завершення програми ===");
        }
    }
}