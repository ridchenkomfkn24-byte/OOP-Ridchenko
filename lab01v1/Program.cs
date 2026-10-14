using System;

namespace lab1v1
{
    public class Book
    {
        // Приватні поля
        private string _title;
        private string _author;
        private int _year;

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Author
        {
            get => _author;
            set => _author = value;
        }

        public int Year
        {
            get => _year;
            set => _year = value;
        }

        public Book(string title, string author, int year)
        {
            _title = title;
            _author = author;
            _year = year;
        }

        ~Book()
        {
            Console.WriteLine($"Об'єкт '{_title}' видалено.");
        }

        public string GetInfo()
        {
            return $"Книга: \"{_title}\" | Автор: {_author} | Рік видання: {_year}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Book book1 = new Book("1984", "Джордж Орвелл", 1949);
            Book book2 = new Book("Інтернат", "Сергій Жадан", 2017);
            Book book3 = new Book("Зазираючи у прірву", "Макс Кідрук", 2023);

            Console.WriteLine(book1.GetInfo());
            Console.WriteLine(book2.GetInfo());
            Console.WriteLine(book3.GetInfo());

            book3.Year = 2024;
            Console.WriteLine($"\nОновлений рік для 3-ї книги: {book3.GetInfo()}");
        }
    }
}