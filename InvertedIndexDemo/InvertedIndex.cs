using System;
using System.Collections.Generic;
using System.Linq;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public override string ToString() => $"[{Id}] {Title}";
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new Dictionary<string, HashSet<int>>();
        private readonly Dictionary<int, Document> _documents = new Dictionary<int, Document>();

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Count;

        /// <summary>Додає документ; наявний ID замінюється з оновленням індексу.</summary>
        public void AddDocument(Document doc)
        {
            ArgumentNullException.ThrowIfNull(doc);
            ArgumentNullException.ThrowIfNull(doc.Content);
            var words = TextProcessor.GetIndexWords(doc.Content).ToHashSet();
            if (_documents.TryGetValue(doc.Id, out Document? previous))
            {
                foreach (string word in TextProcessor.GetIndexWords(previous.Content).Distinct())
                {
                    _index[word].Remove(doc.Id);
                    if (_index[word].Count == 0)
                    {
                        _index.Remove(word);
                    }
                }
            }

            // Зовнішня зміна початкового об'єкта не повинна пошкоджувати індекс.
            _documents[doc.Id] = CopyDocument(doc);
            foreach (string word in words)
            {
                if (!_index.TryGetValue(word, out HashSet<int>? ids))
                {
                    ids = new HashSet<int>();
                    _index.Add(word, ids);
                }
                ids.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            string[] tokens = TextProcessor.GetIndexWords(word).ToArray();
            return tokens.Length == 1 ? GetDocuments(GetIds(tokens[0])) : new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            string[] tokens = NormalizeQuery(words);
            if (tokens.Length == 0)
            {
                return new List<Document>();
            }

            var ids = new HashSet<int>(GetIds(tokens[0]));
            foreach (string word in tokens.Skip(1))
            {
                ids.IntersectWith(GetIds(word));
            }
            return GetDocuments(ids);
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var ids = new HashSet<int>();
            foreach (string word in NormalizeQuery(words))
            {
                ids.UnionWith(GetIds(word));
            }
            return GetDocuments(ids);
        }

        private static string[] NormalizeQuery(string[] words)
        {
            ArgumentNullException.ThrowIfNull(words);
            return words.SelectMany(TextProcessor.GetIndexWords).Distinct().ToArray();
        }

        private IEnumerable<int> GetIds(string word)
        {
            return _index.TryGetValue(word, out HashSet<int>? ids) ? ids : Array.Empty<int>();
        }

        private List<Document> GetDocuments(IEnumerable<int> ids)
        {
            return ids.OrderBy(id => id).Select(id => CopyDocument(_documents[id])).ToList();
        }

        private static Document CopyDocument(Document doc)
        {
            return new Document { Id = doc.Id, Title = doc.Title, Content = doc.Content };
        }
    }
}
