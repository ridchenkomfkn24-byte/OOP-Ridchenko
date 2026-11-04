# Самостійна робота №11 — інвертований індекс
**Студент:** Рідченко Максим, КН-3/1.

Назва проєкту `InvertedIndexDemo` вказана в умові самостійної роботи. Реалізовано три способи пошуку, обробку тексту, порівняння продуктивності та інтерактивний консольний пошук.

## Запуск
З кореня репозиторію, .NET SDK 10:

```powershell
dotnet run -c Release --project InvertedIndexDemo
```

Після демонстрації та вимірювань введи слова для AND-пошуку, наприклад `c# async`. Команда `exit` завершує програму. Для запуску без інтерактивного введення:

```powershell
dotnet run -c Release --project InvertedIndexDemo -- --demo
```

## Приклад виводу
```text
Документів: 5; унікальних слів без стоп-слів: 31
--- Один термін: C# ---
[1] Introduction to C#
[4] C# Advanced Features
[5] Comparing Languages
Знайдено: 3

--- AND: programming language ---
[1] Introduction to C#
[2] Java Basics
[3] Python Overview
Знайдено: 3
```

Звіт, таблиця часу, оцінка порогу вигідності та відповіді — [REPORT.md](REPORT.md). Час залежить від комп’ютера та поточного навантаження.
