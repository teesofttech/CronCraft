---
name: Bug Report
about: Report incorrect output, crashes, or unexpected behavior in CronCraft
title: "[Bug] "
labels: bug
assignees: ''
---

## 🐛 Bug Description

A clear and concise description of what the bug is.

## 🔁 Steps to Reproduce

```csharp
// Paste the minimal code that reproduces the bug
var settings = new CronSettings { Language = "en", DayNameFormat = "short" };
string result = "YOUR_CRON_EXPRESSION".ToHumanReadable(settings);
Console.WriteLine(result);
```

## ✅ Expected Output

What you expected the method to return:

```
Expected: "..."
```

## ❌ Actual Output

What you actually got:

```
Actual: "..."
```

## 🖥️ Environment

- CronCraft version: <!-- e.g. 1.0.2 -->
- .NET version: <!-- e.g. .NET 8.0 -->
- OS: <!-- e.g. Windows 11, Ubuntu 22.04 -->

## 📝 Additional Context

Add any other context, notes, or screenshots here.
