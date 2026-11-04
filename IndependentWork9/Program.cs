using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №9: Dictionary та HashSet ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            Console.WriteLine("--- 1. Кеш метаданих файлів ---");
            var cache = new FileMetadataCache();
            cache.Add("report.txt", new FileMetadata
            {
                FileName = "report.txt",
                Size = 2048,
                LastModified = new DateTime(2026, 11, 4, 12, 0, 0),
                Hash = "demo-hash-001"
            });
            FileMetadata file = cache.Get("REPORT.TXT");
            Console.WriteLine($"Файл: {file.FileName}, розмір: {file.Size} байт");
            Console.WriteLine($"Змінено: {file.LastModified:dd.MM.yyyy HH:mm}, хеш: {file.Hash}");
            Console.WriteLine($"Файл є в кеші: {YesNo(cache.Contains("report.txt"))}");
            cache.Remove("report.txt");
            Console.WriteLine($"Після видалення: {YesNo(cache.Contains("report.txt"))}");
            try
            {
                cache.Get("report.txt");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[ОЧІКУВАНИЙ ВИНЯТОК]: {ex.Message}");
            }

            Console.WriteLine("\n--- 2. Унікальні теги статей ---");
            var tags = new TagManager();
            tags.AddTag("article1", "CSharp");
            tags.AddTag("article1", "csharp");
            tags.AddTag("article1", "NET");
            tags.AddTag("article2", "NET");
            tags.AddTag("article2", "Колекції");
            Console.WriteLine($"Усі унікальні теги: {string.Join(", ", tags.GetAllUniqueTags().OrderBy(t => t, StringComparer.Ordinal))}");
            Console.WriteLine($"Спільні теги: {string.Join(", ", tags.FindCommonTags("article1", "article2"))}");
            Console.WriteLine($"Тільки в першій статті: {string.Join(", ", tags.FindUniqueTags("article1", "article2"))}");
            tags.RemoveTag("article2", "Колекції");
            Console.WriteLine($"Після видалення тегу: {string.Join(", ", tags.GetAllUniqueTags().OrderBy(t => t, StringComparer.Ordinal))}");

            Console.WriteLine("\n--- 3. Пошуковий індекс ---");
            var index = new SearchIndex();
            index.AddDocument(1, "CSharp: колекції, словники та множини.");
            index.AddDocument(2, "Колекції NET. Колекції для пошуку.");
            index.AddDocument(3, "Основи CSharp та ООП.");
            Console.WriteLine($"Пошук «КОЛЕКЦІЇ»: {FormatDocuments(index.Search("КОЛЕКЦІЇ"))}");
            Console.WriteLine($"Пошук «CSharp» і «колекції»: {FormatDocuments(index.SearchMultiple("CSharp", "колекції"))}");
            Console.WriteLine($"Пошук «Java»: {FormatDocuments(index.Search("Java"))}");
            index.AddDocument(2, "Основи програмування.");
            Console.WriteLine($"Після оновлення документа 2, пошук «колекції»: {FormatDocuments(index.Search("колекції"))}");
        }

        static string YesNo(bool value)
        {
            return value ? "Так" : "Ні";
        }

        static string FormatDocuments(HashSet<int> documents)
        {
            return documents.Count == 0 ? "нічого не знайдено" : string.Join(", ", documents.OrderBy(id => id));
        }
    }
}
