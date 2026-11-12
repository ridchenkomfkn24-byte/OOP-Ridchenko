using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace lab18v1
{
    // -----------------------------------------------------------------
    // 1. Власний виняток (InsufficientFundsException)
    // -----------------------------------------------------------------
    [Serializable]
    public class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException()
            : base("Недостатньо коштів на рахунку для виконання операції.")
        {
        }

        public InsufficientFundsException(string message)
            : base(message)
        {
        }

        public InsufficientFundsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InsufficientFundsException(decimal requestedAmount, decimal currentBalance)
            : base($"Недостатньо коштів на рахунку. Запитано: {requestedAmount:N2} грн, поточний баланс: {currentBalance:N2} грн.")
        {
            RequestedAmount = requestedAmount;
            CurrentBalance = currentBalance;
        }

        public InsufficientFundsException(string message, decimal requestedAmount, decimal currentBalance)
            : base(message)
        {
            RequestedAmount = requestedAmount;
            CurrentBalance = currentBalance;
        }

#pragma warning disable SYSLIB0051
        protected InsufficientFundsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RequestedAmount = info.GetDecimal(nameof(RequestedAmount));
            CurrentBalance = info.GetDecimal(nameof(CurrentBalance));
        }
#pragma warning restore SYSLIB0051
    }

    // -----------------------------------------------------------------
    // 2. Узагальнений клас Result<T>
    // -----------------------------------------------------------------
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(default!, false, error);
        }

        public static Result<T> Failure(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            return new Result<T>(default!, false, ex.Message);
        }
    }

    // -----------------------------------------------------------------
    // 3. Сервіс банківських операцій (AccountService)
    // -----------------------------------------------------------------
    public class AccountService
    {
        private decimal _balance;

        public string AccountNumber { get; }
        public decimal Balance => _balance;

        public AccountService(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw new ArgumentException("Номер рахунку не може бути порожнім.", nameof(accountNumber));
            }

            if (initialBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Початковий баланс не може бути від'ємним.");
            }

            AccountNumber = accountNumber;
            _balance = initialBalance;
        }

        /// <summary>
        /// Зняття коштів із рахунку. Викидає InsufficientFundsException, якщо коштів недостатньо.
        /// </summary>
        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Сума зняття повинна бути більшою за 0,00 грн.");
            }

            if (amount > _balance)
            {
                throw new InsufficientFundsException(amount, _balance);
            }

            _balance -= amount;
        }

        /// <summary>
        /// Поповнення рахунку. Повертає Result<decimal> замість викидання винятків для очікуваних помилок валідації.
        /// </summary>
        public Result<decimal> Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return Result<decimal>.Failure($"Некоректна сума поповнення ({amount:N2} грн). Сума повинна бути більшою за 0,00 грн.");
            }

            _balance += amount;
            return Result<decimal>.Success(_balance);
        }

        /// <summary>
        /// Переказ коштів на інший рахунок із перетворенням винятку у Result<decimal>.Failure(Exception).
        /// </summary>
        public Result<decimal> TransferTo(string recipientAccount, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(recipientAccount))
            {
                return Result<decimal>.Failure("Не вказано рахунок отримувача.");
            }

            try
            {
                Withdraw(amount);
                return Result<decimal>.Success(_balance);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Failure(ex);
            }
        }
    }

    // -----------------------------------------------------------------
    // 4. Демонстрація роботи в Main
    // -----------------------------------------------------------------
    internal class Program
    {
        private static readonly CultureInfo UkCulture = new CultureInfo("uk-UA");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = UkCulture;
            CultureInfo.CurrentUICulture = UkCulture;

            Console.WriteLine("=== Лабораторна робота №18: Власні винятки та патерн Result ===");
            Console.WriteLine("Виконав: Рідченко Максим | Група: КН-3/1 | Варіант: 1\n");

            var account = new AccountService("UA213223130000026007233566001", 5000.00m);
            Console.WriteLine($"Відкрито рахунок: {account.AccountNumber}");
            Console.WriteLine($"Початковий баланс: {account.Balance:N2} грн\n");

            // -----------------------------------------------------------------
            // Сценарій 1: Обробка через патерн Result<T> (метод Deposit)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 1: Поповнення рахунку (патерн Result<decimal>) ---");

            // 1.1. Успішне поповнення
            var validDeposit = account.Deposit(2500.00m);
            if (validDeposit.IsSuccess)
            {
                Console.WriteLine($"[УСПІХ] Рахунок поповнено на 2 500,00 грн. Новий баланс: {validDeposit.Value:N2} грн.");
            }
            else
            {
                Console.WriteLine($"[ПОМИЛКА] {validDeposit.Error}");
            }

            // 1.2. Очікувана помилка валідації (сума <= 0)
            var invalidDeposit = account.Deposit(-400.00m);
            if (invalidDeposit.IsSuccess)
            {
                Console.WriteLine($"[УСПІХ] Новий баланс: {invalidDeposit.Value:N2} грн.");
            }
            else
            {
                Console.WriteLine($"[ОЧІКУВАНА ВІДМОВА RESULT] {invalidDeposit.Error}");
            }

            Console.WriteLine("\n-----------------------------------------------------------------\n");

            // -----------------------------------------------------------------
            // Сценарій 2: Обробка власного винятку через try-catch (метод Withdraw)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 2: Зняття коштів (власний виняток InsufficientFundsException) ---");

            // 2.1. Успішне зняття
            try
            {
                decimal withdrawOk = 3000.00m;
                account.Withdraw(withdrawOk);
                Console.WriteLine($"[УСПІХ] Знято {withdrawOk:N2} грн. Залишок на рахунку: {account.Balance:N2} грн.");
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"[ВИНЯТОК] {ex.Message}");
            }

            // 2.2. Спроба зняти суму, що перевищує баланс
            try
            {
                decimal withdrawTooMuch = 10000.00m;
                Console.WriteLine($"Спроба зняти {withdrawTooMuch:N2} грн при балансі {account.Balance:N2} грн...");
                account.Withdraw(withdrawTooMuch);
            }
            catch (InsufficientFundsException ex)
            {
                decimal deficit = ex.RequestedAmount - ex.CurrentBalance;
                Console.WriteLine($"[ПЕРЕХОПЛЕНО InsufficientFundsException]");
                Console.WriteLine($"  * Повідомлення: {ex.Message}");
                Console.WriteLine($"  * Запитана сума: {ex.RequestedAmount:N2} грн");
                Console.WriteLine($"  * Доступний баланс: {ex.CurrentBalance:N2} грн");
                Console.WriteLine($"  * Не вистачає: {deficit:N2} грн");
            }

            Console.WriteLine("\n-----------------------------------------------------------------\n");

            // -----------------------------------------------------------------
            // Сценарій 3: Перетворення винятку у Result<T>.Failure(Exception)
            // -----------------------------------------------------------------
            Console.WriteLine("--- Сценарій 3: Переказ коштів із Result<decimal>.Failure(Exception) ---");

            var transferResult = account.TransferTo("UA903052992990004149123456789", 6000.00m);
            if (transferResult.IsSuccess)
            {
                Console.WriteLine($"[УСПІХ] Переказ виконано. Залишок: {transferResult.Value:N2} грн.");
            }
            else
            {
                Console.WriteLine($"[ПОМИЛКА ПЕРЕКАЗУ] {transferResult.Error}");
            }
        }
    }
}