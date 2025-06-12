
# 🕒 CronCraft

**CronCraft** is a lightweight, developer-friendly .NET SDK that converts cron expressions into human-readable strings — with support for localization, formatting, and time zone awareness — no configuration files required.

---

## 🤔 Why CronCraft?

CronCraft is designed for **clarity, speed, and minimal setup**.

Whether you're building a **web app**, a **CLI tool**, a **Windows Service**, or a **background worker** — just create a `CronSettings` object, and you're good to go.

> ❝ No dependency injection. No appsettings. Just clean, functional cron parsing. ❞

---

## ✨ Features

- 🔄 Converts standard or Quartz cron expressions to plain language
- 🌍 Localized day-of-week formatting (`short`, `full`, `single`, or custom)
- 🕘 Time zone adjustment (optional)
- 🔧 Fully configurable via `CronSettings` object
- 📦 Lightweight, zero-dependency core logic

---

## 📦 Installation

```bash
dotnet add package CronCraft
```

---

## 🛠️ Usage Example

```csharp
using CronCraft.Models;
using CronCraft.Extensions;

string cronExpression = "0 0 * * *"; // Every day at midnight

var settings = new CronSettings
{
    Language = "en",
    DayNameFormat = "short"
};

// Without timezone
string humanReadable = cronExpression.ToHumanReadable(settings);
Console.WriteLine("🔁 CronCraft Expression Translator");
Console.WriteLine("-----------------------------------");
Console.WriteLine($"🧾 Cron Expression:   {cronExpression}");
Console.WriteLine($"📖 Human Readable:    {humanReadable}");
Console.WriteLine("-----------------------------------");
Console.WriteLine("Press Enter to exit...");

// With TimeZone
TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
humanReadable = cronExpression.ToHumanReadable(settings, timeZone);
Console.WriteLine($"📖 Human Readable (Local TZ): {humanReadable}");

Console.ReadLine();
```

**Expected Output:**

```
🔁 CronCraft Expression Translator
-----------------------------------
🧾 Cron Expression:   0 0 * * *
📖 Human Readable:    Every day at 12:00 AM
-----------------------------------
📖 Human Readable (Local TZ): Every day at 01:00 AM
Press Enter to exit...
```

---

## 🔧 Supported Day Name Formats

- `full`: e.g. "Sunday"
- `short`: e.g. "Sun"
- `single`: e.g. "S"
- `custom`: fully user-defined via `Dictionary<string, string>`

---

## 🌐 Languages Supported

- English (`en`)
- Spanish (`es`)
- French (`fr`)
- Easily extendable via custom mappings

---

## 🧑‍💻 Author

**Esanju Babatunde**  
[GitHub](https://github.com/teesofttech) · [LinkedIn](https://www.linkedin.com/in/esanju-babatunde)

---

## 📄 License

MIT
