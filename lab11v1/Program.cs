using System;
using System.Collections.Generic;

namespace lab11v1
{
    // Абстрактний базовий клас із патерном Template Method
    public abstract class DocumentProcessor
    {
        public string FilePath { get; set; }
        public bool IsProcessed { get; private set; }
        public int ElementCount { get; protected set; }

        protected DocumentProcessor(string filePath)
        {
            FilePath = filePath;
            IsProcessed = false;
        }

        // ШАБЛОННИЙ МЕТОД (Template Method)
        // Визначає фіксований скелет алгоритму обробки з 6 послідовних кроків
        public void Process()
        {
            Console.WriteLine($"\n==================================================");
            Console.WriteLine($"[ПОЧАТОК ОБРОБКИ] Файл: '{FilePath}'");
            Console.WriteLine($"==================================================");

            Open();
            Parse();
            
            if (Validate())
            {
                Transform();
                Save();
                IsProcessed = true;
            }
            else
            {
                Console.WriteLine("  [ПОМИЛКА] Валідація документа не пройдена. Подальша обробка скасована.");
            }

            Close();
            Console.WriteLine($"[ФІНАЛ] Статус обробки: {(IsProcessed ? "Успішно" : "Помилка")}");
        }

        // 1. Обов'язкові (абстрактні) кроки — реалізуються в похідних классах
        protected abstract void Open();
        protected abstract void Parse();
        protected abstract void Save();

        // 2. Опціональні (віртуальні) кроки — мають дефолтну реалізацію, але можуть бути перевизначені
        protected virtual bool Validate()
        {
            Console.WriteLine("  [КРОК 3: Validate] Стандартна перевірка: файл існує та не порожній (OK)");
            return true;
        }

        protected virtual void Transform()
        {
            Console.WriteLine("  [КРОК 4: Transform] Стандартна трансформація: кодування переведено в UTF-8");
        }

        protected virtual void Close()
        {
            Console.WriteLine("  [КРОК 6: Close] Закриття файлового дескриптора");
        }
    }

    // 1. Похідний клас: Обробка PDF
    public class PDFProcessor : DocumentProcessor
    {
        public string PdfVersion { get; set; } = "1.7";

        public PDFProcessor(string filePath) : base(filePath) { }

        protected override void Open()
        {
            Console.WriteLine($"  [КРОК 1: Open] Відкриття PDF специфікації v{PdfVersion}...");
        }

        protected override void Parse()
        {
            ElementCount = 12; // Кількість сторінок
            Console.WriteLine($"  [КРОК 2: Parse] Зчитано {ElementCount} сторінок векторної графіки та тексту.");
        }

        protected override bool Validate()
        {
            Console.WriteLine("  [КРОК 3: Validate] Перевірка цифрового підпису та шифрування PDF...");
            return true;
        }

        protected override void Save()
        {
            Console.WriteLine($"  [КРОК 5: Save] Експорт обробленого PDF у сховище.");
        }
    }

    // 2. Похідний клас: Обробка Word
    public class WordProcessor : DocumentProcessor
    {
        public bool TrackChangesEnabled { get; set; } = true;

        public WordProcessor(string filePath) : base(filePath) { }

        protected override void Open()
        {
            Console.WriteLine("  [КРОК 1: Open] Завантаження Word OpenXML структури...");
        }

        protected override void Parse()
        {
            ElementCount = 450; // Кількість слів
            Console.WriteLine($"  [КРОК 2: Parse] Розпарсено {ElementCount} слів та 3 таблиці.");
        }

        protected override void Transform()
        {
            Console.WriteLine("  [КРОК 4: Transform] Очищення стилів, форматування абзаців та видалення приміток.");
        }

        protected override void Save()
        {
            Console.WriteLine("  [КРОК 5: Save] Збереження документа у форматі .docx");
        }
    }

    // 3. Похідний клас: Обробка Excel
    public class ExcelProcessor : DocumentProcessor
    {
        public string ActiveSheet { get; set; } = "Sheet1";

        public ExcelProcessor(string filePath) : base(filePath) { }

        protected override void Open()
        {
            Console.WriteLine($"  [КРОК 1: Open] Відкриття книги Excel, вкладка '{ActiveSheet}'...");
        }

        protected override void Parse()
        {
            ElementCount = 1200; // Кількість клітинок
            Console.WriteLine($"  [КРОК 2: Parse] Прочитано {ElementCount} клітинок із даними.");
        }

        protected override bool Validate()
        {
            Console.WriteLine("  [КРОК 3: Validate] Перевірка цілісності формул та відсутності #REF!");
            return true;
        }

        protected override void Transform()
        {
            Console.WriteLine("  [КРОК 4: Transform] Автоматичний перерахунок формул та сумування підсумків.");
        }

        protected override void Save()
        {
            Console.WriteLine("  [КРОК 5: Save] Експорт таблиці у формат .xlsx");
        }

        protected override void Close()
        {
            base.Close();
            Console.WriteLine("  [КРОК 6 (Extra): Close] Звільнення COM-об'єктів Excel з пам'яті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №11: Патерн Template Method ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1");

            // Створення списку об'єктів базового типу
            List<DocumentProcessor> processors = new List<DocumentProcessor>
            {
                new PDFProcessor("annual_report_2026.pdf"),
                new WordProcessor("lab11_report.docx"),
                new ExcelProcessor("financial_budget.xlsx")
            };

            // Демонстрація поліморфного виклику Шаблонного Методу
            foreach (var processor in processors)
            {
                processor.Process();
            }
        }
    }
}