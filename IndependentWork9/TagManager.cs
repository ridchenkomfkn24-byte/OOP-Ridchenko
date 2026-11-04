using System;
using System.Collections.Generic;

namespace IndependentWork9
{
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags =
            new Dictionary<string, HashSet<string>>();

        public void AddTag(string articleId, string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(articleId);
            ArgumentException.ThrowIfNullOrWhiteSpace(tag);

            if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
            {
                tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _articleTags.Add(articleId, tags);
            }

            tags.Add(tag.Trim());
        }

        public void RemoveTag(string articleId, string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(articleId);
            ArgumentException.ThrowIfNullOrWhiteSpace(tag);
            if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
            {
                tags.Remove(tag.Trim());
            }
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (HashSet<string> tags in _articleTags.Values)
            {
                result.UnionWith(tags);
            }
            return result;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            HashSet<string> result = GetTags(articleId1);
            result.IntersectWith(GetTags(articleId2));
            return result;
        }

        public HashSet<string> FindUniqueTags(string articleId1, string articleId2)
        {
            HashSet<string> result = GetTags(articleId1);
            result.ExceptWith(GetTags(articleId2));
            return result;
        }

        private HashSet<string> GetTags(string articleId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(articleId);
            if (_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
            {
                // Повертаємо копію, щоб операції над множинами не змінювали теги статті.
                return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
            }
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
