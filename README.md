
# 🕒 CronCraft

**CronCraft** is a lightweight, developer-friendly .NET SDK that converts cron expressions into human-readable strings, with support for localization, formatting, and time zone awareness — no configuration files required.

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
using CronCraft.Helper;

var settings = new CronSettings
{
    Language = "en",
    DayNameFormat = "short",
    CustomDayMappings = new Dictionary<string, string>
    {
        { "0", "Sunday" },
        { "1", "Monday" },
        { "2", "Tuesday" },
        { "3", "Wednesday" },
        { "4", "Thursday" },
        { "5", "Friday" },
        { "6", "Saturday" },
        { "7", "Sunday" }
    }
};

string result = CronHelper.ToHumanReadable("30 14 * * *", settings, TimeZoneInfo.Local);
Console.WriteLine(result); // Output: "Chaque jour à 02:30 PM"
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
[GitHub](https://github.com/teesofttech) · [LinkedIn](https://linkedin.com/in/esanju)

---

## 📄 License

MIT
