using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a", "an", "the", "is", "are", "was", "were", "in", "on", "at",
            "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            // Зберігаємо C# і C++, відділяючи іншу пунктуацію.
            return Regex.Matches(text.ToLowerInvariant(), @"[\p{L}\p{N}]+(?:#|\+\+)?")
                .Select(match => match.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            ArgumentNullException.ThrowIfNull(words);
            return words.Where(word => !StopWords.Contains(word));
        }

        public static IEnumerable<string> GetIndexWords(string text)
        {
            return RemoveStopWords(Tokenize(text));
        }
    }
}
