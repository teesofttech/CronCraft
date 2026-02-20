# 🤝 Contributing to CronCraft

Thank you for your interest in contributing to CronCraft! This document describes the open issues and areas where contributors are most welcome. Pick any task that interests you and open a Pull Request.

---

## 🚀 Getting Started

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request 🎉

### Build & Test

```bash
# Build
dotnet build CronCraft.sln

# Run tests
dotnet test CronCraft.Test/CronCraft.Test.csproj
```

---

## 🐛 Open Issues for Contributors

The following issues are waiting to be resolved. Each section describes the problem, the expected behavior, and guidance on where to find the relevant code.

---

### Issue 1 — Add Support for Cron Expression Aliases

**Labels:** `enhancement`, `good first issue`

**Description:**  
CronCraft currently does not recognize shorthand cron aliases such as `@hourly`, `@daily`, `@weekly`, `@monthly`, `@yearly`, and `@reboot`. These are widely used in Unix/Linux cron and many scheduling libraries.

**Expected Behavior:**  
```csharp
"@daily".ToHumanReadable(settings);   // → "Every day at 12:00 AM"
"@hourly".ToHumanReadable(settings);  // → "Every hour"
"@weekly".ToHumanReadable(settings);  // → "Every Sunday at 12:00 AM"
"@monthly".ToHumanReadable(settings); // → "Every month on the 1st at 12:00 AM"
"@yearly".ToHumanReadable(settings);  // → "Every year on January 1st at 12:00 AM"
```

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — `ConvertQuartzToCronos` is a good place to expand with alias normalization before the 5-part parse step.

---

### Issue 2 — Proper Exception Handling Instead of Returning Error Strings

**Labels:** `bug`, `enhancement`

**Description:**  
When an invalid cron expression is given, the library currently returns the string `"Invalid cron expression"` instead of throwing a descriptive exception. This makes it impossible for callers to reliably detect errors without doing string comparison.

**Expected Behavior:**  
An `ArgumentException` (or a custom `InvalidCronExpressionException`) should be thrown with a descriptive message when the cron string is malformed, empty, or has the wrong number of parts.

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs`, line 99 — the `if (parts.Length != 5) return "Invalid cron expression";` check.

---

### Issue 3 — Add Support for Quartz Special Characters: `L`, `W`, `#`

**Labels:** `enhancement`

**Description:**  
Quartz cron expressions support special characters that CronCraft does not yet handle:

| Character | Meaning | Example |
|-----------|---------|---------|
| `L` | Last day of month or week | `0 0 L * ?` → "Last day of the month at 12:00 AM" |
| `W` | Nearest weekday | `0 0 15W * ?` → "Nearest weekday to the 15th at 12:00 AM" |
| `#` | Nth weekday of month | `0 0 ? * 2#1` → "Every first Monday at 12:00 AM" |

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — `BuildHumanReadable` and `ConvertQuartzToCronos` methods.

---

### Issue 4 — Add Support for Range (`1-5`) and List (`1,3,5`) Patterns in All Fields

**Labels:** `enhancement`

**Description:**  
The library only partially handles comma-separated day lists. Range patterns such as `1-5` (e.g., Monday through Friday) are not parsed or described. For example:

- `"0 9 * * 1-5"` should produce `"Every Monday through Friday at 09:00 AM"`.
- `"0 */2 * * 1,3,5"` should produce `"Every 2 hours on Monday, Wednesday and Friday"`.

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — `JoinDays` and `BuildHumanReadable` methods.

---

### Issue 5 — Support Named Weekdays and Months in Cron Expressions

**Labels:** `enhancement`, `good first issue`

**Description:**  
Many cron tools allow named weekdays (`MON`, `TUE`, `WED`, etc.) and named months (`JAN`, `FEB`, etc.) in place of numbers. CronCraft does not currently normalize these before parsing, so expressions like `"0 9 * JAN MON"` would not produce the correct result.

**Expected Behavior:**  
```csharp
"0 9 * JAN MON".ToHumanReadable(settings); // → "Every Monday at 09:00 AM" (in January)
```

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — add a normalization step in `ConvertQuartzToCronos` or at the start of `BuildHumanReadable`.

---

### Issue 6 — Add a 24-Hour Time Format Option

**Labels:** `enhancement`, `good first issue`

**Description:**  
The library always formats times in 12-hour AM/PM format (e.g., `"02:30 PM"`). Many locales and applications prefer 24-hour format (e.g., `"14:30"`). A `TimeFormat` property should be added to `CronSettings`.

**Expected Behavior:**  
```csharp
var settings = new CronSettings { Language = "en", TimeFormat = "HH:mm" };
"30 14 * * *".ToHumanReadable(settings); // → "Every day at 14:30"
```

**Where to Look:**  
- `CronCraft/Models/CronSettings.cs` — add `TimeFormat` property.
- `CronCraft/Extensions/CronHelper.cs` — `FormatTime` method, line 172.

---

### Issue 7 — Add Additional Language Support

**Labels:** `enhancement`, `help wanted`

**Description:**  
CronCraft currently supports English (`en`), Spanish (`es`), and French (`fr`). The community would benefit from additional languages. Good candidates include:

- German (`de`)
- Portuguese (`pt`)
- Italian (`it`)
- Dutch (`nl`)
- Chinese Simplified (`zh`)
- Japanese (`ja`)

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — add a new `ToHumanReadable<Language>` method and phrase dictionary, then register it in the `ToHumanReadable` switch.
- `CronCraft/Providers/DayNameProvider.cs` — add localized day name dictionaries.

---

### Issue 8 — Add a Dependency Injection Extension Method

**Labels:** `enhancement`, `good first issue`

**Description:**  
`CronCraftService` exists but there is no `IServiceCollection` extension method to register it. Contributors should add a `services.AddCronCraft(settings => { })` helper following the standard .NET options pattern.

**Expected Behavior:**  
```csharp
builder.Services.AddCronCraft(settings =>
{
    settings.Language = "en";
    settings.DayNameFormat = "short";
});
```

**Where to Look:**  
- `CronCraft/CronCraftService.cs` — the service to be registered.
- A new file `CronCraft/Extensions/ServiceCollectionExtensions.cs` should be created.

---

### Issue 9 — Expand Test Coverage with Edge Cases and Invalid Inputs

**Labels:** `testing`, `good first issue`

**Description:**  
The current test suite (`CronCraft.Test/CronHelperTest.cs`) covers happy-path scenarios. The following cases are not tested and should be added:

- Empty string input
- `null` input
- Expressions with too few or too many parts (e.g., `"* * *"`)
- Wildcard-only expression `"* * * * *"`
- Invalid field values (e.g., `"*/61 * * * *"` — 61 minutes)
- Spanish and French output correctness
- All four `DayNameFormat` values (`full`, `short`, `single`, `custom`)
- Cron with both a specific day-of-month and a specific day-of-week
- Timezone edge cases (e.g., invalid timezone string)

**Where to Look:**  
- `CronCraft.Test/CronHelperTest.cs`

---

### Issue 10 — Add XML Documentation Comments to All Public Members

**Labels:** `documentation`, `good first issue`

**Description:**  
`CronSettings` and `CronCraftService` lack XML documentation comments. Adding `<summary>`, `<param>`, `<returns>`, and `<example>` comments to all public members will improve IntelliSense and generated API docs.

**Where to Look:**  
- `CronCraft/Models/CronSettings.cs`
- `CronCraft/CronCraftService.cs`

---

### Issue 11 — Add Performance Benchmarks

**Labels:** `enhancement`, `help wanted`

**Description:**  
There are no benchmarks to measure parsing performance. A BenchmarkDotNet project should be added to the solution to track the performance of `ToHumanReadable` across different cron expression types and languages.

**Where to Look:**  
- Create a new `CronCraft.Benchmarks/` project using [BenchmarkDotNet](https://benchmarkdotnet.org/).

---

### Issue 12 — Improve the `month` Field Description

**Labels:** `bug`, `enhancement`

**Description:**  
When a specific month is set (e.g., `"0 9 1 3 *"` — March 1st), the output currently ignores the month entirely and says `"Every month on the 1st at 09:00 AM"`. The month value should be included in the description.

**Expected Behavior:**  
```csharp
"0 9 1 3 *".ToHumanReadable(settings); // → "Every year on March 1st at 09:00 AM"
```

**Where to Look:**  
- `CronCraft/Extensions/CronHelper.cs` — `BuildHumanReadable` method, around line 124.

---

## 📋 How to Claim an Issue

1. Comment on the relevant GitHub issue (or create one referencing this document) to let others know you're working on it.
2. Fork the repo and create your branch.
3. Write tests first where possible (test-driven development is encouraged).
4. Open a Pull Request and reference the issue number.

---

## 🔬 Code Style

- Follow existing patterns in `CronHelper.cs`.
- All public methods must have XML doc comments.
- All new behavior must be covered by unit tests in `CronCraft.Test`.
- Keep new dependencies to a minimum; prefer .NET built-ins.

---

## 📄 License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
