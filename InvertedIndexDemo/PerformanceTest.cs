using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InvertedIndexDemo
{
    public static class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var index = new InvertedIndex();
            var buildWatch = Stopwatch.StartNew();
            foreach (Document doc in documents)
            {
                index.AddDocument(doc);
            }
            buildWatch.Stop();

            List<Document> LinearSearch()
            {
                string[] tokens = TextProcessor.GetIndexWords(searchWord).ToArray();
                return tokens.Length != 1 ? new List<Document>() : documents.Where(doc =>
                    TextProcessor.GetIndexWords(doc.Content).Contains(tokens[0])).ToList();
            }

            // Прогрів і перевірка всіх ID, а не лише кількості збігів.
            List<Document> linear = LinearSearch();
            List<Document> indexed = index.SearchSingleWord(searchWord);
            if (!linear.Select(d => d.Id).OrderBy(id => id).SequenceEqual(indexed.Select(d => d.Id)))
            {
                throw new InvalidOperationException("Результати двох способів пошуку відрізняються.");
            }

            const int repeats = 10;
            double Measure(Func<List<Document>> search)
            {
                var watch = Stopwatch.StartNew();
                for (int i = 0; i < repeats; i++)
                {
                    List<Document> result = search();
                    GC.KeepAlive(result);
                }
                watch.Stop();
                return watch.Elapsed.TotalMilliseconds / repeats;
            }

            double linearMs = Measure(LinearSearch);
            double indexedMs = Measure(() => index.SearchSingleWord(searchWord));
            double savedMs = linearMs - indexedMs;
            string threshold = savedMs > 0 ? Math.Ceiling(buildWatch.Elapsed.TotalMilliseconds / savedMs).ToString("F0") : "немає";
            Console.WriteLine($"| {documents.Count} | {buildWatch.Elapsed.TotalMilliseconds:F4} | {linearMs:F4} | {indexedMs:F4} | {linear.Count} | {threshold} |");
        }

        public static void Run()
        {
            // Прогрів JIT перед вимірюванням побудови індексу.
            var warmup = new InvertedIndex();
            foreach (Document doc in DemoData.CreateDocuments()) warmup.AddDocument(doc);
            warmup.SearchSingleWord("python");
            Console.WriteLine("\nПошук слова python; середнє 10 запитів, час у мс.");
            Console.WriteLine("| Документи | Побудова | Лінійний | Індекс | Збіги | Поріг запитів |\n|---|---|---|---|---|---|");
            foreach (int count in new[] { 10, 100, 1000, 10000 })
            {
                CompareSearchMethods(DemoData.GenerateDocuments(count), "python");
            }
        }
    }
}
