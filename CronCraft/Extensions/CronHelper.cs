using CronCraft.Models;
using CronCraft.Providers;

namespace CronCraft.Extensions;
public static class CronHelper
{
    /// <summary>
    /// To convert a cron expression to a human-readable format based on the specified language and settings.
    /// </summary>
    /// <param name="cronExpression"></param>
    /// <param name="settings"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    public static string ToHumanReadable(this string cronExpression, CronSettings settings, TimeZoneInfo timeZone = null)
    {
        return settings.Language.ToLower() switch
        {
            "es" => ToHumanReadableSpanish(cronExpression, settings, timeZone),
            "fr" => ToHumanReadableFrench(cronExpression, settings, timeZone),
            _ => ToHumanReadableEnglish(cronExpression, settings, timeZone)
        };
    }

    /// <summary>
    /// To convert a cron expression to a human-readable format in English. 
    /// </summary>
    /// <param name="cron"></param>
    /// <param name="settings"></param>
    /// <param name="tz"></param>
    /// <returns></returns>
    private static string ToHumanReadableEnglish(string cron, CronSettings settings, TimeZoneInfo tz)
    {
        return BuildHumanReadable(cron, settings, tz, new Dictionary<string, string>
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

    /// <summary>
    /// To convert a cron expression to a human-readable format in Spanish.
    /// </summary>
    /// <param name="cron"></param>
    /// <param name="settings"></param>
    /// <param name="tz"></param>
    /// <returns></returns>
    private static string ToHumanReadableSpanish(string cron, CronSettings settings, TimeZoneInfo tz)
    {
        return BuildHumanReadable(cron, settings, tz, new Dictionary<string, string>
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

    /// <summary>
    /// To convert a cron expression to a human-readable format in French.
    /// </summary>
    /// <param name="cron"></param>
    /// <param name="settings"></param>
    /// <param name="tz"></param>
    /// <returns></returns>
    private static string ToHumanReadableFrench(string cron, CronSettings settings, TimeZoneInfo tz)
    {
        return BuildHumanReadable(cron, settings, tz, new Dictionary<string, string>
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

    /// <summary>
    /// Builds a human-readable string from a cron expression using the specified settings and time zone.
    /// </summary>
    /// <param name="cron"></param>
    /// <param name="settings"></param>
    /// <param name="timeZone"></param>
    /// <param name="phrases"></param>
    /// <returns></returns>
    private static string BuildHumanReadable(string cron, CronSettings settings, TimeZoneInfo timeZone, Dictionary<string, string> phrases)
    {
        var daysMap = DayNameProvider.GetDayMap(settings);
        string[] parts = ConvertQuartzToCronos(cron).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5) return "Invalid cron expression";

        string minute = parts[0], hour = parts[1], dayOfMonth = parts[2], month = parts[3], dayOfWeek = parts[4];
        string time = FormatTime(hour, minute, timeZone);

        string Phrase(string key, params object[] args) =>
            phrases.TryGetValue(key, out var value) ? string.Format(value, args) : string.Format(key, args);

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
            string monthDesc = month.StartsWith("*/") ? $"Every {month.Replace("*/", "")} months" : "Every month";
            return $"{monthDesc} on the {Ordinal(dayOfMonth)} {Phrase("AtTime", time)}";
        }

        if (dayOfMonth != "*" && dayOfWeek != "*")
            return Phrase("OnDayAndWeek", Ordinal(dayOfMonth), JoinDays(dayOfWeek, daysMap)) + " " + Phrase("AtTime", time);

        return cron;
    }

    #region [Private Methods]

    /// <summary>
    /// Converts a Quartz cron expression to a Cronos-compatible expression.
    /// </summary>
    /// <param name="quartzCron"></param>
    /// <returns></returns>
    private static string ConvertQuartzToCronos(string quartzCron)
    {
        string[] parts = quartzCron.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 6 || parts.Length == 7)
        {
            string minute = parts[1];
            string hour = parts[2];
            string day = parts[3];
            string month = parts[4];
            string dayOfWeek = parts[5] == "?" ? "*" : parts[5];

            return $"{minute} {hour} {day} {month} {dayOfWeek}";
        }
        return quartzCron;
    }

    /// <summary>
    /// Formats the time based on the hour and minute, adjusting for the specified time zone.
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    private static string FormatTime(string hour, string minute, TimeZoneInfo timeZone)
    {
        int h = int.TryParse(hour.Replace("*/", "0"), out int hParsed) ? hParsed : 0;
        int m = int.TryParse(minute.Replace("*/", "0"), out int mParsed) ? mParsed : 0;
        DateTime utcTime = new DateTime(2000, 1, 1, h, m, 0, DateTimeKind.Utc);
        DateTime localTime = timeZone != null ? TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone) : utcTime;
        return localTime.ToString("hh:mm tt");
    }

    /// <summary>
    /// Converts a number to its ordinal representation (1st, 2nd, 3rd, etc.).
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static string Ordinal(string number)
    {
        if (number.Contains("/"))
            number = number.Split('/')[0];

        if (!int.TryParse(number, out int n))
            return number;

        return n switch
        {
            int _ when (n % 100 >= 11 && n % 100 <= 13) => $"{n}th",
            int _ when n % 10 == 1 => $"{n}st",
            int _ when n % 10 == 2 => $"{n}nd",
            int _ when n % 10 == 3 => $"{n}rd",
            _ => $"{n}th",
        };
    }

    /// <summary>
    /// Checks if the provided day of the week string represents all days of the week.
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    private static bool IsAllDays(string dayOfWeek)
    {
        HashSet<string> all = new HashSet<string> { "0", "1", "2", "3", "4", "5", "6" };
        HashSet<string> input = dayOfWeek.Split(',').ToHashSet();
        return input.SetEquals(all);
    }

    /// <summary>
    /// Joins the days of the week into a human-readable format.
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
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

    #endregion [Private Methods]
}
