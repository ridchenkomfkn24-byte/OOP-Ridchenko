using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork11
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Enumerable.Empty<string>();

            var matches = Regex.Matches(text.ToLowerInvariant(), @"\b[a-z0-9#+]+\b");
            var result = new List<string>();

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "a", "an", "the", "is", "are", "was", "were",
                "in", "on", "at", "to", "for", "of", "and", "or"
            };

            return words.Where(w => !stopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _documents = new();

        public void AddDocument(Document doc)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            _documents[doc.Id] = doc;
            var words = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));

            foreach (var word in words)
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }
                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalizedWord = word.Trim().ToLowerInvariant();
            if (_index.TryGetValue(normalizedWord, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            HashSet<int>? resultSet = null;

            foreach (var rawWord in words)
            {
                var normalizedWord = rawWord.Trim().ToLowerInvariant();
                var docIds = _index.TryGetValue(normalizedWord, out var ids) ? ids : new HashSet<int>();

                if (resultSet == null)
                {
                    resultSet = new HashSet<int>(docIds);
                }
                else
                {
                    resultSet.IntersectWith(docIds);
                }
            }

            return resultSet != null
                ? resultSet.Select(id => _documents[id]).ToList()
                : new List<Document>();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            var resultSet = new HashSet<int>();

            foreach (var rawWord in words)
            {
                var normalizedWord = rawWord.Trim().ToLowerInvariant();
                if (_index.TryGetValue(normalizedWord, out var ids))
                {
                    resultSet.UnionWith(ids);
                }
            }

            return resultSet.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();
            var normalizedWord = searchWord.ToLowerInvariant();

            // 1. Лінійний пошук
            sw.Start();
            var linearResults = documents.Where(d =>
                TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            // 2. Індексований пошук
            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine($"[Документів: {documents.Count}]");
            Console.WriteLine($"  - Лінійний пошук:  {linearTicks} ticks (знайдено: {linearResults.Count})");
            Console.WriteLine($"  - Інвертований індекс: {indexTicks} ticks (знайдено: {indexResults.Count})");
            Console.WriteLine($"  - Прискорення: {(double)linearTicks / Math.Max(indexTicks, 1):F2}x\n");
        }

        public static List<Document> GenerateDocuments(int count)
        {
            var baseDocs = GetBaseDocuments();
            var list = new List<Document>(count);
            var random = new Random(42);

            for (int i = 1; i <= count; i++)
            {
                var template = baseDocs[(i - 1) % baseDocs.Count];
                list.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Title} #{i}",
                    Content = $"{template.Content} Extra tag_{random.Next(1, 100)}"
                });
            }

            return list;
        }

        public static List<Document> GetBaseDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Самостійна робота №11: Інвертований індекс та вимірювання продуктивності ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1\n");

            Console.WriteLine("=== ТЕСТ ПРОДУКТИВНОСТІ (Stopwatch) ===");
            int[] testSizes = { 10, 100, 1000, 10000 };
            foreach (var size in testSizes)
            {
                var docs = PerformanceTest.GenerateDocuments(size);
                PerformanceTest.CompareSearchMethods(docs, "programming");
            }

            Console.WriteLine("=== ІНТЕРАКТИВНИЙ ПОШУК ===");
            var index = new InvertedIndex();
            var baseDocs = PerformanceTest.GetBaseDocuments();

            foreach (var doc in baseDocs)
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Завантажено документів: {index.DocumentCount}");
            Console.WriteLine($"Унікальних слів у індексі: {index.UniqueWords}\n");

            while (true)
            {
                Console.Write("Введіть пошуковий запит (або 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Trim().ToLowerInvariant() == "exit") break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\n--- Результати пошуку (AND) ---");
                var results = index.SearchMultipleWords(words);

                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($" {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }
                Console.WriteLine($"\nЗнайдено документів: {results.Count}\n");
            }
        }
    }
}