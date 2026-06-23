using CronCraft.Exceptions;
using CronCraft.Models;
using CronCraft.Providers;

namespace CronCraft.Extensions;

public static class CronHelper
{
    /// <summary>
    /// Converts a cron expression to a human-readable format based on language and settings.
    /// </summary>
    /// <param name="cronExpression">The cron expression to convert.</param>
    /// <param name="settings">The settings controlling language, day-name format, and timezone.</param>
    /// <param name="timeZone">Optional timezone to convert UTC times into.</param>
    /// <returns>A human-readable description of the cron schedule.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="cronExpression"/> is null or empty.</exception>
    /// <exception cref="InvalidCronExpressionException">Thrown when <paramref name="cronExpression"/> is malformed.</exception>
    public static string ToHumanReadable(this string cronExpression, CronSettings settings, TimeZoneInfo? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            throw new ArgumentException("Cron expression must not be null or empty.", nameof(cronExpression));

        return settings.Language.ToLower() switch
        {
            "es" => ToHumanReadableSpanish(cronExpression, settings, timeZone),
            "fr" => ToHumanReadableFrench(cronExpression, settings, timeZone),
            "de" => ToHumanReadableGerman(cronExpression, settings, timeZone),
            "pt" => ToHumanReadablePortuguese(cronExpression, settings, timeZone),
            "it" => ToHumanReadableItalian(cronExpression, settings, timeZone),
            "nl" => ToHumanReadableDutch(cronExpression, settings, timeZone),
            "zh" => ToHumanReadableChinese(cronExpression, settings, timeZone),
            "ja" => ToHumanReadableJapanese(cronExpression, settings, timeZone),
            _ => ToHumanReadableEnglish(cronExpression, settings, timeZone)
        };
    }

    private static string ToHumanReadableEnglish(string cron, CronSettings settings, TimeZoneInfo? timeZone)
    {
        return BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Every day",
            ["AtTime"] = "at {0}",
            ["EveryXMinutes"] = "Every {0} minutes",
            ["EveryXHours"] = "Every {0} hours",
            ["EveryXHoursOn"] = "Every {0} hours on {1}",
            ["EveryMonthOnDay"] = "Every month on the {0}",
            ["OnDayAndWeek"] = "On {0} and {1}",
            ["EveryDayOfWeek"] = "Every {0}",
            ["EveryXMonthsOnDay"] = "Every {0} months on the {1}",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableSpanish(string cron, CronSettings settings, TimeZoneInfo? timeZone)
    {
        return BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Cada día",
            ["AtTime"] = "a las {0}",
            ["EveryXMinutes"] = "Cada {0} minutos",
            ["EveryXHours"] = "Cada {0} horas",
            ["EveryXHoursOn"] = "Cada {0} horas los {1}",
            ["EveryMonthOnDay"] = "Cada mes el día {0}",
            ["OnDayAndWeek"] = "El {0} y los {1}",
            ["EveryDayOfWeek"] = "Cada {0}",
            ["EveryXMonthsOnDay"] = "Cada {0} meses el día {1}",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableFrench(string cron, CronSettings settings, TimeZoneInfo? timeZone)
    {
        return BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Chaque jour",
            ["AtTime"] = "à {0}",
            ["EveryXMinutes"] = "Toutes les {0} minutes",
            ["EveryXHours"] = "Toutes les {0} heures",
            ["EveryXHoursOn"] = "Toutes les {0} heures le {1}",
            ["EveryMonthOnDay"] = "Chaque mois le {0}",
            ["OnDayAndWeek"] = "Le {0} et le {1}",
            ["EveryDayOfWeek"] = "Chaque {0}",
            ["EveryXMonthsOnDay"] = "Tous les {0} mois le {1}",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableGerman(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Jeden Tag",
            ["AtTime"] = "um {0} Uhr",
            ["EveryXMinutes"] = "Alle {0} Minuten",
            ["EveryXHours"] = "Alle {0} Stunden",
            ["EveryXHoursOn"] = "Alle {0} Stunden am {1}",
            ["EveryMonthOnDay"] = "Jeden Monat am {0}",
            ["OnDayAndWeek"] = "Am {0} und am {1}",
            ["EveryDayOfWeek"] = "Jeden {0}",
            ["EveryXMonthsOnDay"] = "Alle {0} Monate am {1}",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadablePortuguese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Todos os dias",
            ["AtTime"] = "às {0}",
            ["EveryXMinutes"] = "A cada {0} minutos",
            ["EveryXHours"] = "A cada {0} horas",
            ["EveryXHoursOn"] = "A cada {0} horas em {1}",
            ["EveryMonthOnDay"] = "Todo mês no dia {0}",
            ["OnDayAndWeek"] = "No dia {0} e em {1}",
            ["EveryDayOfWeek"] = "Em {0}",
            ["EveryXMonthsOnDay"] = "A cada {0} meses no dia {1}",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableItalian(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Ogni giorno",
            ["AtTime"] = "alle {0}",
            ["EveryXMinutes"] = "Ogni {0} minuti",
            ["EveryXHours"] = "Ogni {0} ore",
            ["EveryXHoursOn"] = "Ogni {0} ore di {1}",
            ["EveryMonthOnDay"] = "Ogni mese il giorno {0}",
            ["OnDayAndWeek"] = "Il giorno {0} e di {1}",
            ["EveryDayOfWeek"] = "Ogni {0}",
            ["EveryXMonthsOnDay"] = "Ogni {0} mesi il giorno {1}",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableDutch(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Elke dag",
            ["AtTime"] = "om {0}",
            ["EveryXMinutes"] = "Elke {0} minuten",
            ["EveryXHours"] = "Elke {0} uur",
            ["EveryXHoursOn"] = "Elke {0} uur op {1}",
            ["EveryMonthOnDay"] = "Elke maand op de {0}",
            ["OnDayAndWeek"] = "Op de {0} en op {1}",
            ["EveryDayOfWeek"] = "Elke {0}",
            ["EveryXMonthsOnDay"] = "Elke {0} maanden op de {1}",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableChinese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "每天",
            ["AtTime"] = "{0}",
            ["EveryXMinutes"] = "每 {0} 分钟",
            ["EveryXHours"] = "每 {0} 小时",
            ["EveryXHoursOn"] = "每 {0} 小时（{1}）",
            ["EveryMonthOnDay"] = "每月第 {0} 天",
            ["OnDayAndWeek"] = "每月第 {0} 天和{1}",
            ["EveryDayOfWeek"] = "每{0}",
            ["EveryXMonthsOnDay"] = "每 {0} 个月的第 {1} 天",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableJapanese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "毎日",
            ["AtTime"] = "{0}に",
            ["EveryXMinutes"] = "{0}分ごと",
            ["EveryXHours"] = "{0}時間ごと",
            ["EveryXHoursOn"] = "{1}に{0}時間ごと",
            ["EveryMonthOnDay"] = "毎月{0}日",
            ["OnDayAndWeek"] = "毎月{0}日と{1}",
            ["EveryDayOfWeek"] = "毎週{0}",
            ["EveryXMonthsOnDay"] = "{0}か月ごとの{1}日",
            ["TimeFormat"] = "24"
        });

    private static string BuildHumanReadable(
        string cron,
        CronSettings settings,
        TimeZoneInfo? timeZone,
        Dictionary<string, string> phrases)
    {
        var daysMap = DayNameProvider.GetDayMap(settings);

        string[] parts = ConvertQuartzToCronos(cron)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 5)
            throw new InvalidCronExpressionException(cron, $"expected 5 parts but got {parts.Length}");

        string minute = parts[0];
        string hour = parts[1];
        string dayOfMonth = parts[2];
        string month = parts[3];
        string dayOfWeek = parts[4];

        string time = FormatTime(hour, minute, timeZone, phrases);

        string Phrase(string key, params object[] args) =>
            phrases.TryGetValue(key, out var value)
                ? string.Format(value, args)
                : string.Format(key, args);

        // --- Quartz special characters: L, W, # ---

        // day-of-week: 2#1 → "Every first Monday of the month"
        // Quartz day-of-week is 1-based (1=Sun…7=Sat); daysMap is 0-based (0=Sun…6=Sat)
        if (dayOfWeek.Contains('#'))
        {
            var nthResult = DescribeNthWeekday(dayOfWeek, daysMap);
            return $"{nthResult} {Phrase("AtTime", time)}";
        }

        // day-of-week: 2L → "Last Monday of the month" (Quartz 1-based, convert to 0-based)
        if (dayOfWeek.EndsWith('L') && dayOfWeek != "L")
        {
            var quartzDay = int.TryParse(dayOfWeek.TrimEnd('L'), out var qd) ? qd : -1;
            var standardDay = (quartzDay - 1).ToString();
            var dayName = daysMap.TryGetValue(standardDay, out var n) ? n : $"Day {standardDay}";
            return $"Last {dayName} of the month {Phrase("AtTime", time)}";
        }

        // day-of-month: LW → "Last weekday of the month"
        if (dayOfMonth.Equals("LW", StringComparison.OrdinalIgnoreCase))
            return $"Last weekday of the month {Phrase("AtTime", time)}";

        // day-of-month: L → "Last day of the month"
        if (dayOfMonth == "L")
            return $"Last day of the month {Phrase("AtTime", time)}";

        // day-of-month: 15W → "Nearest weekday to the 15th"
        if (dayOfMonth.EndsWith('W'))
        {
            var baseDay = Ordinal(dayOfMonth.TrimEnd('W'), settings.Language);
            return $"Nearest weekday to the {baseDay} of the month {Phrase("AtTime", time)}";
        }

        // --- Range and list patterns ---

        if (TryDescribeTimePattern(hour, minute, timeZone, phrases, out var timePattern))
        {
            if (dayOfWeek != "*" && dayOfWeek != "?")
                return $"{timePattern} on {JoinDays(dayOfWeek, daysMap)}";

            if (dayOfMonth != "*")
                return $"{timePattern} on the {DescribeOrdinals(dayOfMonth, settings.Language)} of every month";

            return timePattern.StartsWith("At ", StringComparison.Ordinal)
                ? $"Every day at {timePattern[3..]}"
                : timePattern;
        }

        if (dayOfMonth.Contains('-') && (dayOfWeek == "*" || dayOfWeek == "?"))
        {
            var dayRange = DescribeOrdinals(dayOfMonth, settings.Language);
            return $"Every day from the {dayRange} {Phrase("AtTime", time)}";
        }

        if (minute.StartsWith("*/") && hour == "*")
            return Phrase("EveryXMinutes", minute.Replace("*/", ""));

        if (hour.StartsWith("*/") && minute == "0")
        {
            var interval = hour.Replace("*/", "");
            return dayOfWeek != "*" && !IsAllDays(dayOfWeek)
                ? Phrase("EveryXHoursOn", interval, JoinDays(dayOfWeek, daysMap))
                : Phrase("EveryXHours", interval);
        }

        if ((dayOfWeek == "*" || dayOfWeek == "?") && dayOfMonth == "*")
            return Phrase("EveryDay") + " " + Phrase("AtTime", time);

        if (dayOfWeek != "*" && dayOfMonth == "*")
            return $"{Phrase("EveryDayOfWeek", JoinDays(dayOfWeek, daysMap))} {Phrase("AtTime", time)}";

        if (dayOfMonth != "*" && (dayOfWeek == "*" || dayOfWeek == "?"))
        {
            var ordinal = DescribeOrdinals(dayOfMonth, settings.Language);

            return month.StartsWith("*/")
                ? $"{Phrase("EveryXMonthsOnDay", month.Replace("*/", ""), ordinal)} {Phrase("AtTime", time)}"
                : $"{Phrase("EveryMonthOnDay", ordinal)} {Phrase("AtTime", time)}";
        }

        if (dayOfMonth != "*" && dayOfWeek != "*")
            return Phrase("OnDayAndWeek", DescribeOrdinals(dayOfMonth, settings.Language), JoinDays(dayOfWeek, daysMap))
                   + " " + Phrase("AtTime", time);

        return cron;
    }

    // "2#1" → "Every first Monday of the month"  (Quartz day 2 = Mon in 1-based, so subtract 1)
    // "6#3" → "Every third Friday of the month"
    private static string DescribeNthWeekday(string dayOfWeek, Dictionary<string, string> daysMap)
    {
        var hashParts = dayOfWeek.Split('#');
        // Quartz uses 1-based day-of-week; daysMap is 0-based
        var standardDay = int.TryParse(hashParts[0], out var qd) ? (qd - 1).ToString() : hashParts[0];
        var dayName = daysMap.TryGetValue(standardDay, out var n) ? n : $"Day {standardDay}";
        string occurrence = hashParts[1] switch
        {
            "1" => "first",
            "2" => "second",
            "3" => "third",
            "4" => "fourth",
            "5" => "fifth",
            _ => $"{hashParts[1]}th"
        };
        return $"Every {occurrence} {dayName} of the month";
    }

    private static string ConvertQuartzToCronos(string quartzCron)
    {
        string[] parts = quartzCron.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 6 || parts.Length == 7)
        {
            string minute = parts[1];
            string hour = parts[2];
            string day = parts[3];
            string month = parts[4];
            // Preserve L/#-suffixed values; only replace a bare "?" with "*"
            string dayOfWeek = parts[5] == "?" ? "*" : parts[5];

            return $"{minute} {hour} {day} {month} {dayOfWeek}";
        }

        return quartzCron;
    }

    private static string FormatTime(
        string hour,
        string minute,
        TimeZoneInfo? timeZone,
        Dictionary<string, string> phrases)
    {
        int h = int.TryParse(hour.Replace("*/", "0"), out var hParsed) ? hParsed : 0;
        int m = int.TryParse(minute.Replace("*/", "0"), out var mParsed) ? mParsed : 0;

        return FormatTime(h, m, timeZone, phrases);
    }

    private static string FormatTime(
        int hour,
        int minute,
        TimeZoneInfo? timeZone,
        Dictionary<string, string> phrases)
    {
        DateTime utcTime = new DateTime(2000, 1, 1, hour, minute, 0, DateTimeKind.Utc);

        DateTime localTime = timeZone != null
            ? TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone)
            : utcTime;

        var format = phrases.TryGetValue("TimeFormat", out var timeFormat) && timeFormat == "24"
            ? "HH:mm"
            : "hh:mm tt";
        return localTime.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string Ordinal(string number, string language = "en")
    {
        if (number.Contains("/"))
            number = number.Split('/')[0];

        if (!int.TryParse(number, out int n))
            return number;

        return language.ToLowerInvariant() switch
        {
            "de" => $"{n}.",
            "pt" => $"{n}º",
            "it" or "zh" or "ja" => n.ToString(),
            "nl" => $"{n}e",
            _ => EnglishOrdinal(n)
        };
    }

    private static string EnglishOrdinal(int n)
    {
        return n switch
        {
            int _ when n % 100 is >= 11 and <= 13 => $"{n}th",
            int _ when n % 10 == 1 => $"{n}st",
            int _ when n % 10 == 2 => $"{n}nd",
            int _ when n % 10 == 3 => $"{n}rd",
            _ => $"{n}th",
        };
    }

    private static string DescribeOrdinals(string expression, string language = "en")
    {
        var descriptions = expression
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part =>
            {
                var bounds = part.Split('-', StringSplitOptions.TrimEntries);
                return bounds.Length == 2
                    ? $"{Ordinal(bounds[0], language)} through the {Ordinal(bounds[1], language)}"
                    : Ordinal(part, language);
            })
            .ToList();

        return JoinDescriptions(descriptions);
    }

    private static bool TryDescribeTimePattern(
        string hour,
        string minute,
        TimeZoneInfo? timeZone,
        Dictionary<string, string> phrases,
        out string description)
    {
        description = string.Empty;

        if (hour.Contains('-') && int.TryParse(minute, out var fixedMinute))
        {
            var bounds = hour.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length == 2 &&
                int.TryParse(bounds[0], out var startHour) &&
                int.TryParse(bounds[1], out var endHour))
            {
                description =
                    $"Every hour from {FormatTime(startHour, fixedMinute, timeZone, phrases)} to {FormatTime(endHour, fixedMinute, timeZone, phrases)}";
                return true;
            }
        }

        if (minute.Contains('-') && int.TryParse(hour, out var fixedHour))
        {
            var bounds = minute.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length == 2 &&
                int.TryParse(bounds[0], out var startMinute) &&
                int.TryParse(bounds[1], out var endMinute))
            {
                description =
                    $"Every minute from {FormatTime(fixedHour, startMinute, timeZone, phrases)} to {FormatTime(fixedHour, endMinute, timeZone, phrases)}";
                return true;
            }
        }

        if ((hour.Contains(',') || minute.Contains(',')) &&
            TryExpandValues(hour, out var hours) &&
            TryExpandValues(minute, out var minutes))
        {
            var times = (
                from parsedHour in hours
                from parsedMinute in minutes
                select FormatTime(parsedHour, parsedMinute, timeZone, phrases))
                .ToList();

            description = $"At {JoinDescriptions(times)}";
            return true;
        }

        return false;
    }

    private static bool TryExpandValues(string expression, out List<int> values)
    {
        values = [];

        foreach (var part in expression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var bounds = part.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length == 1 && int.TryParse(bounds[0], out var value))
            {
                values.Add(value);
                continue;
            }

            if (bounds.Length == 2 &&
                int.TryParse(bounds[0], out var start) &&
                int.TryParse(bounds[1], out var end) &&
                start <= end)
            {
                values.AddRange(Enumerable.Range(start, end - start + 1));
                continue;
            }

            values = [];
            return false;
        }

        return values.Count > 0;
    }

    private static bool IsAllDays(string dayOfWeek)
    {
        HashSet<string> all = new() { "0", "1", "2", "3", "4", "5", "6" };
        HashSet<string> input = ExpandDayValues(dayOfWeek)
            .Select(day => day == "7" ? "0" : day)
            .ToHashSet();
        return input.SetEquals(all);
    }

    private static string JoinDays(string dayOfWeek, Dictionary<string, string> daysMap)
    {
        var days = dayOfWeek
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => DescribeDayPart(part, daysMap))
            .Distinct()
            .ToList();

        return JoinDescriptions(days, "days");
    }

    private static string DescribeDayPart(string part, Dictionary<string, string> daysMap)
    {
        var bounds = part.Split('-', StringSplitOptions.TrimEntries);
        if (bounds.Length == 2)
        {
            return $"{GetDayName(bounds[0], daysMap)} through {GetDayName(bounds[1], daysMap)}";
        }

        return GetDayName(part, daysMap);
    }

    private static string GetDayName(string day, Dictionary<string, string> daysMap) =>
        daysMap.TryGetValue(day, out var name) ? name : $"Day {day}";

    private static IEnumerable<string> ExpandDayValues(string expression)
    {
        foreach (var part in expression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var bounds = part.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length == 2 &&
                int.TryParse(bounds[0], out var start) &&
                int.TryParse(bounds[1], out var end) &&
                start <= end)
            {
                foreach (var day in Enumerable.Range(start, end - start + 1))
                    yield return day.ToString();
            }
            else
            {
                yield return part;
            }
        }
    }

    private static string JoinDescriptions(IReadOnlyList<string> descriptions, string fallback = "")
    {
        return descriptions.Count switch
        {
            > 2 => string.Join(", ", descriptions.Take(descriptions.Count - 1)) + " and " + descriptions[^1],
            2 => descriptions[0] + " and " + descriptions[1],
            1 => descriptions[0],
            _ => fallback,
        };
    }
}
