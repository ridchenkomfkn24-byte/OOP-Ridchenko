using System;

namespace Lab3v1
{
    public class FileLogger : IDisposable
    {
        private bool _disposed = false;
        private bool _isFileOpen;
        private string _filePath;

        public string FilePath => _filePath;
        public bool IsFileOpen => _isFileOpen;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
            _isFileOpen = true;
            Console.WriteLine($"[FileLogger] Файл '{_filePath}' відкрито для запису.");
        }

        public void Log(string message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FileLogger), "Неможливо виконати логування: об'єкт вже знищено.");
            }

            if (_isFileOpen)
            {
                Console.WriteLine($"[LOG to {_filePath}]: {message}");
            }
            else
            {
                Console.WriteLine($"[ERROR]: Спроба запису у закритий файл '{_filePath}'.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)] Звільнення керованих ресурсів для '{_filePath}'.");
                }

                if (_isFileOpen)
                {
                    _isFileOpen = false;
                    Console.WriteLine($"[Dispose] Файл '{_filePath}' успішно закрито.");
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileLogger()
        {
            Console.WriteLine($"[~FileLogger] Працює деструктор (фіналізатор) для '{_filePath}'.");
            Dispose(false);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== СЦЕНАРІЙ 1: Використання конструкції 'using' ===");
            ScenarioUsing();

            Console.WriteLine("\n=== СЦЕНАРІЙ 2: Явний виклик Dispose() ===");
            ScenarioExplicitDispose();

            Console.WriteLine("\n=== СЦЕНАРІЙ 3: Неявна фіналізація через GC (без Dispose) ===");
            ScenarioGarbageCollector();

            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nРоботу програми завершено.");
        }

        private static void ScenarioUsing()
        {
            using (var logger = new FileLogger("app_using.log"))
            {
                logger.Log("Повідомлення 1 (всередині using)");
                logger.Log("Повідомлення 2 (всередині using)");
            }
        }

        private static void ScenarioExplicitDispose()
        {
            var logger = new FileLogger("app_explicit.log");
            logger.Log("Повідомлення 1 (перед Dispose)");
            
            logger.Dispose();
            logger.Dispose();
        }

        private static void ScenarioGarbageCollector()
        {
            CreateUnreferencedLogger();

            Console.WriteLine("Примусовий виклик GC.Collect()...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void CreateUnreferencedLogger()
        {
            var logger = new FileLogger("app_gc.log");
            logger.Log("Повідомлення (об'єкт буде зібрано GC)");
        }
    }
}