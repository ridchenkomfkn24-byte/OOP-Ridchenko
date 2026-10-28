using System;
using System.Collections.Generic;

namespace lab14v1
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";

        public override string ToString()
        {
            return $"[ID: {Id}] {Title} (Пріоритет: {Priority})";
        }
    }

    public class TaskManager
    {
        private readonly HashSet<string> _tags = new(StringComparer.OrdinalIgnoreCase);
        private readonly Queue<TaskItem> _executionQueue = new();
        private readonly Stack<TaskItem> _completedHistory = new();

        public void AddTag(string tag)
        {
            if (_tags.Add(tag))
            {
                Console.WriteLine($"[HashSet] Тег '{tag}' успішно додано.");
            }
            else
            {
                Console.WriteLine($"[HashSet] Тег '{tag}' ВЖЕ ІСНУЄ! Дублікат відхилено.");
            }
        }

        public void DisplayTags()
        {
            Console.WriteLine($"[HashSet] Унікальні теги ({_tags.Count} шт.): " + string.Join(", ", _tags));
        }

        public void AddTaskToQueue(TaskItem task)
        {
            _executionQueue.Enqueue(task);
            Console.WriteLine($"[Queue] Завдання додано в чергу: {task}");
        }

        public void ProcessNextTask()
        {
            if (_executionQueue.Count == 0)
            {
                Console.WriteLine("[Queue] Черга завдань порожня!");
                return;
            }

            Console.WriteLine($"[Queue Peek] Наступне на виконання: {_executionQueue.Peek()}");
            TaskItem processedTask = _executionQueue.Dequeue();
            Console.WriteLine($"[Queue Dequeue] Виконано завдання: {processedTask}");
            _completedHistory.Push(processedTask);
        }

        public void UndoLastCompletedTask()
        {
            if (_completedHistory.Count == 0)
            {
                Console.WriteLine("[Stack] Історія порожня. Скасування неможливе!");
                return;
            }

            Console.WriteLine($"[Stack Peek] Останнє виконане завдання: {_completedHistory.Peek()}");
            TaskItem undoneTask = _completedHistory.Pop();
            Console.WriteLine($"[Stack Pop (Undo)] Скасовано виконання завдання: {undoneTask}");
            _executionQueue.Enqueue(undoneTask);
        }

        public void DisplayState()
        {
            Console.WriteLine($"\n--- ПОТОЧНИЙ СТАН СИСТЕМИ ---");
            Console.WriteLine($"Завдань у черзі (Queue): {_executionQueue.Count}");
            Console.WriteLine($"Виконаних у історії (Stack): {_completedHistory.Count}");
            Console.WriteLine("-----------------------------\n");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №14: Практичне використання HashSet, Queue, Stack ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            TaskManager manager = new TaskManager();

            Console.WriteLine("=== 1. РОБОТА З HASHSET (ТЕГИ) ===");
            manager.AddTag("Backend");
            manager.AddTag("BugFix");
            manager.AddTag("backend");
            manager.AddTag("UI/UX");
            manager.DisplayTags();
            Console.WriteLine();

            Console.WriteLine("=== 2. РОБОТА З QUEUE (ЧЕРГА ЗАВДАНЬ - FIFO) ===");
            manager.AddTaskToQueue(new TaskItem { Id = 101, Title = "Виправити авторизацію", Priority = "High" });
            manager.AddTaskToQueue(new TaskItem { Id = 102, Title = "Оновити базову БД", Priority = "Medium" });
            manager.AddTaskToQueue(new TaskItem { Id = 103, Title = "Верстка головної сторінки", Priority = "Low" });

            manager.DisplayState();

            Console.WriteLine("Обробка перших 2-х завдань із черги:");
            manager.ProcessNextTask();
            manager.ProcessNextTask();

            manager.DisplayState();

            Console.WriteLine("=== 3. РОБОТА З STACK (ІСТОРІЯ ВІДМІНИ - LIFO) ===");
            manager.UndoLastCompletedTask();

            manager.DisplayState();
        }
    }
}