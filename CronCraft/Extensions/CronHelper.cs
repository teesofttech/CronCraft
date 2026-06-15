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
            ["OnDayAndWeek"] = "On {0} and {1}"
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
            ["OnDayAndWeek"] = "El {0} y los {1}"
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
            ["OnDayAndWeek"] = "Le {0} et le {1}"
        });
    }

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

        string time = FormatTime(hour, minute, timeZone);

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
            var baseDay = Ordinal(dayOfMonth.TrimEnd('W'));
            return $"Nearest weekday to the {baseDay} of the month {Phrase("AtTime", time)}";
        }

        // --- Standard patterns ---

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
            return $"Every {JoinDays(dayOfWeek, daysMap)} {Phrase("AtTime", time)}";

        if (dayOfMonth != "*" && (dayOfWeek == "*" || dayOfWeek == "?"))
        {
            string monthDesc = month.StartsWith("*/")
                ? $"Every {month.Replace("*/", "")} months"
                : "Every month";

            return $"{monthDesc} on the {Ordinal(dayOfMonth)} {Phrase("AtTime", time)}";
        }

        if (dayOfMonth != "*" && dayOfWeek != "*")
            return Phrase("OnDayAndWeek", Ordinal(dayOfMonth), JoinDays(dayOfWeek, daysMap))
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

    private static string FormatTime(string hour, string minute, TimeZoneInfo? timeZone)
    {
        int h = int.TryParse(hour.Replace("*/", "0"), out var hParsed) ? hParsed : 0;
        int m = int.TryParse(minute.Replace("*/", "0"), out var mParsed) ? mParsed : 0;

        DateTime utcTime = new DateTime(2000, 1, 1, h, m, 0, DateTimeKind.Utc);

        DateTime localTime = timeZone != null
            ? TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone)
            : utcTime;

        return localTime.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string Ordinal(string number)
    {
        if (number.Contains("/"))
            number = number.Split('/')[0];

        if (!int.TryParse(number, out int n))
            return number;

        return n switch
        {
            int _ when n % 100 is >= 11 and <= 13 => $"{n}th",
            int _ when n % 10 == 1 => $"{n}st",
            int _ when n % 10 == 2 => $"{n}nd",
            int _ when n % 10 == 3 => $"{n}rd",
            _ => $"{n}th",
        };
    }

    private static bool IsAllDays(string dayOfWeek)
    {
        HashSet<string> all = new() { "0", "1", "2", "3", "4", "5", "6" };
        HashSet<string> input = dayOfWeek.Split(',').ToHashSet();
        return input.SetEquals(all);
    }

    private static string JoinDays(string dayOfWeek, Dictionary<string, string> daysMap)
    {
        var days = dayOfWeek
            .Split(',')
            .Select(d => daysMap.TryGetValue(d, out var name) ? name : $"Day {d}")
            .Distinct()
            .ToList();

        return days.Count switch
        {
            > 2 => string.Join(", ", days.Take(days.Count - 1)) + " and " + days.Last(),
            2 => days[0] + " and " + days[1],
            1 => days[0],
            _ => "days",
        };
    }
}
