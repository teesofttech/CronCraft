
# 🕒 CronCraft

**CronCraft** is a lightweight, developer-friendly .NET SDK that converts cron expressions into human-readable strings — with support for localization, formatting, and time zone awareness — no configuration files required.

---

## 🤔 Why CronCraft?

CronCraft is designed for **clarity, speed, and minimal setup**.

Whether you're building a **web app**, a **CLI tool**, a **Windows Service**, or a **background worker** — just create a `CronSettings` object, and you're good to go.

> ❝ No dependency injection required. No appsettings. Just clean, functional cron parsing. ❞

---

## ✨ Features

- 🔄 Converts standard or Quartz cron expressions to plain language
- 🌍 Localized day-of-week formatting (`short`, `full`, `single`, or custom)
- 🕘 Time zone adjustment (optional)
- 🔧 Fully configurable via `CronSettings` object
- 💉 Optional dependency injection registration with `AddCronCraft`
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
    DayNameFormat = "short",
    TimeFormat = "HH:mm" // Optional: 24-hour output (default: "hh:mm tt")
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

### Dependency Injection

For ASP.NET Core and generic host applications, register CronCraft through
`IServiceCollection`:

```csharp
using CronCraft.Extensions;

builder.Services.AddCronCraft(settings =>
{
    settings.Language = "en";
    settings.DayNameFormat = "short";
});
```

Then inject `CronCraftService` where it is needed:

```csharp
public class MyScheduler(CronCraftService cronCraft)
{
    public string Describe(string expression) => cronCraft.Convert(expression);
}
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

## ✅ Validation and Non-Throwing Conversion

`ToHumanReadable` continues to throw `ArgumentException`,
`InvalidCronExpressionException`, or timezone exceptions when invalid input is
exceptional:

```csharp
try
{
    string description = cronExpression.ToHumanReadable(settings);
}
catch (InvalidCronExpressionException ex)
{
    Console.WriteLine(ex.ValidationError.Code);
}
```

Interactive applications can use the non-throwing API instead:

```csharp
if (cronExpression.TryToHumanReadable(
        settings,
        out var description,
        out var error))
{
    Console.WriteLine(description);
}
else
{
    Console.WriteLine($"{error!.Code}: {error.Message}");
}

bool valid = cronExpression.IsValid(settings);
bool detailed = cronExpression.TryValidate(out var validationError, settings);
```

Dependency-injection users can call `TryConvert`, `TryValidate`, and `IsValid`
on `CronCraftService`.

Stable error codes are available through `CronValidationErrorCodes`:

| Code | Meaning |
|------|---------|
| `null_expression` | The expression reference is null |
| `empty_expression` | The expression is empty or whitespace |
| `invalid_part_count` | The expression has an unsupported field count |
| `invalid_field_value` | A cron field contains an invalid value |
| `invalid_range` | A range is malformed, out of bounds, or reversed |
| `unsupported_syntax` | CronCraft does not support the supplied syntax |
| `invalid_time_zone` | The configured timezone is unavailable |
| `invalid_settings` | Another formatting or mapping setting is invalid |

---

## 🔧 Supported Day Name Formats

- `full`: e.g. "Sunday"
- `short`: e.g. "Sun"
- `single`: e.g. "S"
- `custom`: fully user-defined via `Dictionary<string, string>`

---

## 📅 Named Weekdays and Months

Weekday and month abbreviations are case-insensitive and work in individual,
list, and range expressions:

```csharp
"0 9 * JAN MON".ToHumanReadable(settings);
// Every Mon in January at 09:00 AM

"0 9 * * MON-FRI".ToHumanReadable(settings);
// Every Mon through Fri at 09:00 AM

"0 0 1 JAN,JUL *".ToHumanReadable(settings);
// Every year on January 1st and July 1st at 12:00 AM
```

Supported weekday names are `SUN` through `SAT`; supported month names are
`JAN` through `DEC`.

---

## ⚡ Cron Aliases

Common Unix cron aliases can be used instead of five-part expressions:

```csharp
"@hourly".ToHumanReadable(settings);  // Every hour
"@daily".ToHumanReadable(settings);   // Every day at 12:00 AM
"@weekly".ToHumanReadable(settings);  // Every Sun at 12:00 AM
"@monthly".ToHumanReadable(settings); // Every month on the 1st at 12:00 AM
"@yearly".ToHumanReadable(settings);  // Every year on January 1st at 12:00 AM
"@reboot".ToHumanReadable(settings);  // At system reboot
```

Aliases are case-insensitive and produce localized descriptions using the
selected `CronSettings.Language`.

---

## ⏱️ Quartz Seconds and Years

Quartz expressions may contain six fields (including seconds) or seven fields
(including an optional year):

```csharp
"*/10 * * * * ?".ToHumanReadable(settings);
// Every 10 seconds

"30 15 10 * * ?".ToHumanReadable(settings);
// Every day at 10:15:30 AM

"0 0 12 * * ? 2027-2029".ToHumanReadable(settings);
// Every day at 12:00 PM in 2027 through 2029
```

Seconds support fixed values, wildcards, steps, lists, and ranges from `0` to
`59`. A non-zero fixed second is added to the rendered time even when
`TimeFormat` omits `ss`; formats that already contain seconds are preserved.

Years support values, steps, lists, and ranges from `1970` to `2099`. A `*`
year remains unrestricted and is omitted from the description. Five-field Unix
cron expressions keep their existing behavior.

---

## 🌐 Languages Supported

- English (`en`)
- Spanish (`es`)
- French (`fr`)
- German (`de`)
- Portuguese (`pt`)
- Italian (`it`)
- Dutch (`nl`)
- Simplified Chinese (`zh`)
- Japanese (`ja`)
- Easily extendable via custom mappings

Localized descriptions use language-specific ordinals, conjunctions, ranges,
and singular forms:

```csharp
var french = new CronSettings { Language = "fr", DayNameFormat = "short" };

string description = "0 9 * * 1,3,5".ToHumanReadable(french);
// Chaque lun, mer et ven à 09:00 AM
```

---

## 🧑‍💻 Author

**Esanju Babatunde**  
[GitHub](https://github.com/teesofttech) · [LinkedIn](https://www.linkedin.com/in/esanju-babatunde)

---

## 📊 Performance Benchmarks

CronCraft includes a [BenchmarkDotNet](https://benchmarkdotnet.org/) project to measure parsing performance across expression types and languages.

**Run the benchmarks:**

```bash
cd CronCraft.Benchmarks
dotnet run -c Release -- --filter *
```

**Sample results** (Apple M-series, .NET 8):

| Benchmark | Mean | Allocated |
|-----------|------|-----------|
| Simple (`*/5 * * * *`) | ~1 µs | minimal |
| With day list (`0 */2 * * 1,2,3,4,5`) | ~2 µs | minimal |
| Quartz 6-part | ~1.5 µs | minimal |
| With timezone | ~2 µs | minimal |
| Spanish | ~1.5 µs | minimal |

> Tip: add `--filter *Simple*` to run a single benchmark by name.

---

## 🤝 Contributing

Contributions are welcome! If you'd like to:

- Fix a bug
- Improve performance
- Add support for a new language
- Refactor or improve documentation

Feel free to open a [Pull Request](https://github.com/teesofttech/CronCraft/pulls) or start a [Discussion](https://github.com/teesofttech/CronCraft/discussions).

Before contributing:

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your message'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a pull request 🚀

If you're not sure where to start, check the [issues](https://github.com/teesofttech/CronCraft/issues) or the [CONTRIBUTING guide](CONTRIBUTING.md) for a full list of open tasks.

Let's make cron expressions human again! ❤️

---

## 📄 License

This project is licensed under the MIT License.  
See the [LICENSE](LICENSE) file for details.
