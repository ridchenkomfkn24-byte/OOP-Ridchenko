using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab10v1
{
    // ----------------------------------------------------
    // ЧАСТИНА 1: Інтерфейси та сутності (IRepository<T>)
    // ----------------------------------------------------

    // Сутність користувача
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[ID: {Id}] {Name} ({Email})";
        }
    }

    // Дженерик-інтерфейс репозиторію (Контракт)
    public interface IRepository<T>
    {
        T? GetById(int id);
        void Add(T item);
        void Update(T item);
        void Delete(int id);
        IEnumerable<T> GetAll();
    }

    // Реалізація 1: Збереження у оперативній пам'яті (In-Memory)
    public class InMemoryUserRepository : IRepository<User>
    {
        private readonly List<User> _users = new();

        public void Add(User item)
        {
            _users.Add(item);
            Console.WriteLine($"[InMemoryRepository] Додано користувача: {item.Name}");
        }

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() => _users;

        public void Update(User item)
        {
            var existing = GetById(item.Id);
            if (existing != null)
            {
                existing.Name = item.Name;
                existing.Email = item.Email;
                Console.WriteLine($"[InMemoryRepository] Оновлено користувача з ID {item.Id}");
            }
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _users.Remove(user);
                Console.WriteLine($"[InMemoryRepository] Видалено користувача з ID {id}");
            }
        }
    }

    // Реалізація 2: Збереження у текстовому файлі (File-Based)
    public class FileUserRepository : IRepository<User>
    {
        private readonly string _filePath = "users_db.txt";

        public FileUserRepository()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath); // Очищення для демонстрації
            }
        }

        public void Add(User item)
        {
            string line = $"{item.Id};{item.Name};{item.Email}";
            File.AppendAllLines(_filePath, new[] { line });
            Console.WriteLine($"[FileRepository] Записано у файл '{_filePath}': {item.Name}");
        }

        public User? GetById(int id) => GetAll().FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll()
        {
            if (!File.Exists(_filePath)) return Enumerable.Empty<User>();

            var lines = File.ReadAllLines(_filePath);
            var result = new List<User>();

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 3 && int.TryParse(parts[0], out int id))
                {
                    result.Add(new User { Id = id, Name = parts[1], Email = parts[2] });
                }
            }

            return result;
        }

        public void Update(User item)
        {
            var users = GetAll().ToList();
            var index = users.FindIndex(u => u.Id == item.Id);
            if (index != -1)
            {
                users[index] = item;
                SaveAll(users);
                Console.WriteLine($"[FileRepository] Оновлено у файлі користувача з ID {item.Id}");
            }
        }

        public void Delete(int id)
        {
            var users = GetAll().Where(u => u.Id != id).ToList();
            SaveAll(users);
            Console.WriteLine($"[FileRepository] Видалено з файлу користувача з ID {id}");
        }

        private void SaveAll(List<User> users)
        {
            var lines = users.Select(u => $"{u.Id};{u.Name};{u.Email}");
            File.WriteAllLines(_filePath, lines);
        }
    }


    // ----------------------------------------------------
    // ЧАСТИНА 2: Абстрактний клас (DocumentProcessor)
    // ----------------------------------------------------

    // Абстрактний клас з Шаблонним методом (Template Method)
    public abstract class DocumentProcessor
    {
        public string DocumentName { get; set; }

        protected DocumentProcessor(string documentName)
        {
            DocumentName = documentName;
        }

        // Конкретний (спільний) метод базового класу
        public void LogProcessStart()
        {
            Console.WriteLine($"\n[LOG] Початок обробки документа '{DocumentName}'...");
        }

        // Шаблонний алгоритм, що використовує конкретні та абстрактні методи
        public void Process()
        {
            LogProcessStart();
            ReadContent();
            TransformData();
            SaveResult();
            Console.WriteLine($"[LOG] Обробку '{DocumentName}' завершено успішно.");
        }

        // Абстрактні методи (контракт для похідних класів)
        protected abstract void ReadContent();
        protected abstract void TransformData();
        protected abstract void SaveResult();
    }

    // 1. Похідний клас: Обробник PDF
    public class PdfDocumentProcessor : DocumentProcessor
    {
        public PdfDocumentProcessor(string name) : base(name) { }

        protected override void ReadContent() => Console.WriteLine("  -> Зчитування бінарної структури PDF-файлу");
        protected override void TransformData() => Console.WriteLine("  -> Конвертація PDF-сторінок у текстові блоки");
        protected override void SaveResult() => Console.WriteLine("  -> Збереження результату у форматі .pdf.txt");
    }

    // 2. Похідний клас: Обробник Word
    public class WordDocumentProcessor : DocumentProcessor
    {
        public WordDocumentProcessor(string name) : base(name) { }

        protected override void ReadContent() => Console.WriteLine("  -> Парсинг OpenXML структури .docx документа");
        protected override void TransformData() => Console.WriteLine("  -> Форматування стилів та абзаців тексту");
        protected override void SaveResult() => Console.WriteLine("  -> Збереження у форматі .docx");
    }

    // 3. Похідний клас: Обробник Excel
    public class ExcelDocumentProcessor : DocumentProcessor
    {
        public ExcelDocumentProcessor(string name) : base(name) { }

        protected override void ReadContent() => Console.WriteLine("  -> Зчитування таблиць та клітинок із XLSX");
        protected override void TransformData() => Console.WriteLine("  -> Перерахунок формул та сумування рядків");
        protected override void SaveResult() => Console.WriteLine("  -> Експорт результатів у .csv");
    }


    // ----------------------------------------------------
    // ЧАСТИНА 3: Головна програма (Main)
    // ----------------------------------------------------

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №10: Абстрактні класи та інтерфейси ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            // ====================================================
            // ДЕМОНСТРАЦІЯ 1: Робота з інтерфейсом IRepository<T>
            // ====================================================
            Console.WriteLine("=== 1. ДЕМОНСТРАЦІЯ ІНТЕРФЕЙСІВ (IRepository<User>) ===");

            var repositories = new List<IRepository<User>>
            {
                new InMemoryUserRepository(),
                new FileUserRepository()
            };

            User testUser1 = new User { Id = 1, Name = "Іван Петренко", Email = "ivan@example.com" };
            User testUser2 = new User { Id = 2, Name = "Олена Коваль", Email = "olena@example.com" };

            // Поліморфний виклик методів інтерфейсу
            foreach (var repo in repositories)
            {
                Console.WriteLine($"\n--> Використання репозиторію типу: {repo.GetType().Name}");
                repo.Add(testUser1);
                repo.Add(testUser2);

                Console.WriteLine("Список користувачів у репозиторії:");
                foreach (var user in repo.GetAll())
                {
                    Console.WriteLine($"  {user}");
                }
            }

            // ====================================================
            // ДЕМОНСТРАЦІЯ 2: Робота з абстрактним класом DocumentProcessor
            // ====================================================
            Console.WriteLine("\n=== 2. ДЕМОНСТРАЦІЯ АБСТРАКТНОГО КЛАСУ (DocumentProcessor) ===");

            var processors = new List<DocumentProcessor>
            {
                new PdfDocumentProcessor("Звіт_2026.pdf"),
                new WordDocumentProcessor("Договір_Оренди.docx"),
                new ExcelDocumentProcessor("Бюджет_КН31.xlsx")
            };

            // Поліморфний виклик методів абстрактного класу
            foreach (var processor in processors)
            {
                processor.Process(); // Виклик шаблонного алгоритму
            }
        }
    }
}