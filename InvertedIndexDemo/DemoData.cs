using System.Collections.Generic;
using System.Linq;

namespace InvertedIndexDemo
{
    public static class DemoData
    {
        public static List<Document> CreateDocuments()
        {
            return new List<Document>
            {
                new Document { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new Document { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new Document { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new Document { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new Document { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };
        }

        public static List<Document> GenerateDocuments(int count)
        {
            List<Document> templates = CreateDocuments();
            return Enumerable.Range(1, count).Select(id => new Document
            {
                Id = id,
                Title = templates[(id - 1) % templates.Count].Title + $" ({id})",
                Content = templates[(id - 1) % templates.Count].Content
            }).ToList();
        }
    }
}
