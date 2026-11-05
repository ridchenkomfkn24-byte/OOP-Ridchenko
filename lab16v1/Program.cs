using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace lab16v1
{
    // ----------------------------------------------------
    // Сутності (Entities)
    // ----------------------------------------------------
    public class Chapter
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PageCount { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }

    // ----------------------------------------------------
    // Custom Extension Method (Метод розширення)
    // ----------------------------------------------------
    public static class ChapterExtensions
    {
        /// <summary>
        /// Обчислює загальну кількість сторінок для послідовності розділів.
        /// </summary>
        public static int TotalPages(this IEnumerable<Chapter> chapters)
        {
            if (chapters == null)
            {
                throw new ArgumentNullException(nameof(chapters));
            }

            return chapters.Sum(c => c.PageCount);
        }
    }

    internal class Program
    {
        private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = UkCulture;
            CultureInfo.CurrentUICulture = UkCulture;

            Console.WriteLine("=== Лабораторна робота №16: LINQ SelectMany, пагінація й custom extensions ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            // Ініціалізація тестових даних (Автори -> Книги -> Розділи)
            var authors = GetSampleAuthors();

            // ----------------------------------------------------
            // Запит 1: SelectMany (Розгортання вкладених колекцій)
            // ----------------------------------------------------
            Console.WriteLine("--- 1. Запит SelectMany: Отримання всіх розділів усіх книг усіх авторів ---");

            var allChapters = authors
                .SelectMany(author => author.Books)
                .SelectMany(book => book.Chapters)
                .ToList();

            int index = 1;
            foreach (var chapter in allChapters)
            {
                Console.WriteLine($"{index++}. Розділ: \"{chapter.Title}\" ({chapter.PageCount} стор.)");
            }

            // ----------------------------------------------------
            // Запит 2: Пагінація книг (Skip + Take)
            // ----------------------------------------------------
            Console.WriteLine("\n----------------------------------------------------\n");
            Console.WriteLine("--- 2. Пагінація: Виведення всіх книг (по 3 книги на сторінку) ---");

            var allBooks = authors.SelectMany(author => author.Books).ToList();
            int pageSize = 3;
            int totalPages = (int)Math.Ceiling((double)allBooks.Count / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n=== Сторінка {page} з {totalPages} ===");

                var currentPageBooks = allBooks
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                foreach (var book in currentPageBooks)
                {
                    Console.WriteLine($"  * [ID: {book.Id}] Книга: \"{book.Title}\" (Кількість розділів: {book.Chapters.Count})");
                }
            }

            // ----------------------------------------------------
            // Запит 3: Використання Custom Extension Method
            // ----------------------------------------------------
            Console.WriteLine("\n----------------------------------------------------\n");
            Console.WriteLine("--- 3. Custom Extension Method: Обчислення TotalPages() ---");

            // Загальна кількість сторінок усіх розділів усіх авторів
            int grandTotalPages = allChapters.TotalPages();
            Console.WriteLine($"Загальна кількість сторінок у всіх розділах бібліотеки: {grandTotalPages} стор.");

            // Сторінки для книг конкретного автора
            var firstAuthor = authors.First();
            var firstAuthorChapters = firstAuthor.Books.SelectMany(b => b.Chapters);
            Console.WriteLine($"Загальна кількість сторінок у розділах автора {firstAuthor.Name}: {firstAuthorChapters.TotalPages()} стор.");
        }

        private static List<Author> GetSampleAuthors()
        {
            return new List<Author>
            {
                new Author
                {
                    Id = 1,
                    Name = "Тарас Шевченко",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = 101,
                            Title = "Кобзар",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 1, Title = "Думи мої, думи мої", PageCount = 12 },
                                new Chapter { Id = 2, Title = "Катерина", PageCount = 45 },
                                new Chapter { Id = 3, Title = "Гайдамаки", PageCount = 80 }
                            }
                        },
                        new Book
                        {
                            Id = 102,
                            Title = "Три літа",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 4, Title = "Сон", PageCount = 25 },
                                new Chapter { Id = 5, Title = "Кавказ", PageCount = 18 }
                            }
                        }
                    }
                },
                new Author
                {
                    Id = 2,
                    Name = "Іван Франко",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = 103,
                            Title = "Захар Беркут",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 6, Title = "Вступ та знайомство з громадою", PageCount = 30 },
                                new Chapter { Id = 7, Title = "Лови на ведмедя", PageCount = 40 },
                                new Chapter { Id = 8, Title = "Битва в тухольській долині", PageCount = 55 }
                            }
                        },
                        new Book
                        {
                            Id = 104,
                            Title = "Украдене щастя",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 9, Title = "Дія перша", PageCount = 35 },
                                new Chapter { Id = 10, Title = "Дія друга", PageCount = 42 }
                            }
                        }
                    }
                },
                new Author
                {
                    Id = 3,
                    Name = "Леся Українка",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = 105,
                            Title = "Лісова пісня",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 11, Title = "Пролог", PageCount = 15 },
                                new Chapter { Id = 12, Title = "Дія I: Весна", PageCount = 50 },
                                new Chapter { Id = 13, Title = "Дія II: Літо", PageCount = 48 }
                            }
                        },
                        new Book
                        {
                            Id = 106,
                            Title = "Бояриня",
                            Chapters = new List<Chapter>
                            {
                                new Chapter { Id = 14, Title = "Сцена в Києві", PageCount = 28 },
                                new Chapter { Id = 15, Title = "Сцена в Москві", PageCount = 32 }
                            }
                        }
                    }
                }
            };
        }
    }
}