using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork9
{
    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex =
            new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            ArgumentNullException.ThrowIfNull(content);

            // Повторне додавання документа замінює його попередній вміст.
            foreach (string word in _invertedIndex.Keys.ToList())
            {
                _invertedIndex[word].Remove(docId);
                if (_invertedIndex[word].Count == 0)
                {
                    _invertedIndex.Remove(word);
                }
            }

            foreach (Match match in Regex.Matches(content, @"[\p{L}\p{N}]+"))
            {
                string word = match.Value;
                if (!_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
                {
                    documents = new HashSet<int>();
                    _invertedIndex.Add(word, documents);
                }
                documents.Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(word);
            if (_invertedIndex.TryGetValue(word.Trim(), out HashSet<int>? documents))
            {
                return new HashSet<int>(documents);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            ArgumentNullException.ThrowIfNull(words);
            if (words.Length == 0)
            {
                return new HashSet<int>();
            }

            HashSet<int> result = Search(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                result.IntersectWith(Search(words[i]));
            }
            return result;
        }
    }
}
