using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab9v1
{
    // Базовий абстрактний клас сповіщення
    public abstract class Notification
    {
        private string _recipient = string.Empty;
        private string _message = string.Empty;

        public string Recipient
        {
            get => _recipient;
            set
            {
                ValidateRecipient(value);
                _recipient = value;
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Повідомлення не може бути порожнім.");
                }
                _message = value;
            }
        }

        protected abstract void ValidateRecipient(string recipient);
        public abstract void Send();
    }

    // 1. Email сповіщення
    public class EmailNotification : Notification
    {
        public string Subject { get; set; } = "Без теми";

        protected override void ValidateRecipient(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient) || !Regex.IsMatch(recipient, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException($"Некоректний Email формат адресата: '{recipient}'");
            }
        }

        public override void Send()
        {
            Console.WriteLine($"[EMAIL] Надсилання листа на {Recipient} | Тема: '{Subject}' | Повідомлення: '{Message}'");
        }
    }

    // 2. SMS сповіщення
    public class SMSNotification : Notification
    {
        protected override void ValidateRecipient(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient) || !Regex.IsMatch(recipient, @"^\+380\d{9}$"))
            {
                throw new ArgumentException($"Некоректний телефонний номер (очікується формат +380XXXXXXXXX): '{recipient}'");
            }
        }

        public override void Send()
        {
            Console.WriteLine($"[SMS] Надсилання SMS на {Recipient} | Повідомлення: '{Message}'");
        }
    }

    // 3. Push сповіщення
    public class PushNotification : Notification
    {
        public string DeviceId { get; set; } = string.Empty;

        protected override void ValidateRecipient(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient) || recipient.Length < 8)
            {
                throw new ArgumentException($"Некоректний Device ID (має бути не менше 8 символів): '{recipient}'");
            }
        }

        public override void Send()
        {
            Console.WriteLine($"[PUSH] Надсилання Push-сповіщення на пристрій {Recipient} | Повідомлення: '{Message}'");
        }
    }

    // Сервіс розсилки сповіщень
    public class NotificationService
    {
        public void SendAll(List<Notification> notifications)
        {
            Console.WriteLine("=== РОЗПОЧАТО ПРОЦЕС РОЗСИЛКИ СПОВІЩЕНЬ ===\n");
            int successCount = 0;
            int failureCount = 0;

            foreach (var notification in notifications)
            {
                try
                {
                    notification.Send(); // Поліморфний виклик
                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    Console.WriteLine($"[ПОМИЛКА ВІДПРАВКИ] Не вдалося відправити сповіщення для {notification.GetType().Name}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n=== ПІДСУМОК: Успішно: {successCount} | З помилками: {failureCount} ===");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №9: Поліморфні сервіси ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант 1\n");

            var notifications = new List<Notification>();

            // Коректні об'єкти
            try
            {
                notifications.Add(new EmailNotification
                {
                    Recipient = "user@example.com",
                    Subject = "Вітаємо!",
                    Message = "Дякуємо за реєстрацію в системі."
                });

                notifications.Add(new SMSNotification
                {
                    Recipient = "+380501234567",
                    Message = "Ваш код підтвердження: 4829"
                });

                notifications.Add(new PushNotification
                {
                    Recipient = "DEV-987654321-X",
                    Message = "У вас 1 нове повідомлення."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА СТВОРЕННЯ]: {ex.Message}");
            }

            // Навмисне створення об'єкта з невалідними даними для демонстрації обробки помилок
            Console.WriteLine("--- Спроба додати невалідні об'єкти для перевірки валідації ---");
            
            // 1. Помилка валідації номера SMS
            try
            {
                var badSms = new SMSNotification
                {
                    Recipient = "0501234567", // Неправильний формат без +380
                    Message = "Тестове SMS"
                };
                notifications.Add(badSms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПЕРЕХОПЛЕНО ПОМИЛКУ ВАЛІДАЦІЇ SMS]: {ex.Message}");
            }

            // 2. Помилка валідації Email
            try
            {
                var badEmail = new EmailNotification
                {
                    Recipient = "invalid-email-address",
                    Message = "Тестовий лист"
                };
                notifications.Add(badEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПЕРЕХОПЛЕНО ПОМИЛКУ ВАЛІДАЦІЇ EMAIL]: {ex.Message}");
            }

            Console.WriteLine("\n------------------------------------------------");

            // Виклик сервісу розсилки
            NotificationService service = new NotificationService();
            service.SendAll(notifications);
        }
    }
}