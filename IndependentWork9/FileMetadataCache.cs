using System;
using System.Collections.Generic;

namespace IndependentWork9
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache =
            new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string path, FileMetadata metadata)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(metadata);
            _cache[path] = metadata;
        }

        public FileMetadata Get(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (_cache.TryGetValue(path, out FileMetadata? metadata))
            {
                return metadata;
            }

            throw new KeyNotFoundException($"Файл '{path}' відсутній у кеші.");
        }

        public bool Remove(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            return _cache.Remove(path);
        }

        public bool Contains(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            return _cache.ContainsKey(path);
        }
    }
}
