using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InvertedIndexDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Console.WriteLine("=== Самостійна робота №11: Інвертований індекс ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1");
            var index = new InvertedIndex();
            foreach (Document doc in DemoData.CreateDocuments()) index.AddDocument(doc);
            Console.WriteLine($"Документів: {index.DocumentCount}; унікальних слів без стоп-слів: {index.UniqueWords}");
            PrintResults("Один термін: C#", index.SearchSingleWord("C#"));
            PrintResults("AND: programming language", index.SearchMultipleWords("programming", "language"));
            PrintResults("OR: java python", index.SearchAnyWord("java", "python"));
            PerformanceTest.Run();
            if (args.Contains("--demo")) return;

            while (true)
            {
                Console.Write("\nВведи запит (AND), або exit: ");
                string? query = Console.ReadLine();
                if (query == null || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
                if (string.IsNullOrWhiteSpace(query)) continue;
                PrintResults("Результати пошуку", index.SearchMultipleWords(query));
            }
        }

        static void PrintResults(string title, List<Document> results)
        {
            Console.WriteLine($"\n--- {title} ---");
            foreach (Document doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine("  " + doc.Content);
            }
            Console.WriteLine($"Знайдено: {results.Count}");
        }
    }
}
