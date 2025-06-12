using CronCraft.Models;

namespace CronCraft.Providers;

public static class DayNameProvider
{
    private static readonly Dictionary<string, string> Single = new()
    {
        { "0", "S" }, { "1", "M" }, { "2", "T" }, { "3", "W" },
        { "4", "T" }, { "5", "F" }, { "6", "S" }, { "7", "S" }
    };

    private static readonly Dictionary<string, string> Short = new()
    {
        { "0", "Sun" }, { "1", "Mon" }, { "2", "Tue" }, { "3", "Wed" },
        { "4", "Thu" }, { "5", "Fri" }, { "6", "Sat" }, { "7", "Sun" }
    };

    private static readonly Dictionary<string, string> Full = new()
    {
        { "0", "Sunday" }, { "1", "Monday" }, { "2", "Tuesday" }, { "3", "Wednesday" },
        { "4", "Thursday" }, { "5", "Friday" }, { "6", "Saturday" }, { "7", "Sunday" }
    };

    public static Dictionary<string, string> GetDayMap(CronSettings settings)
    {
        if (settings.DayNameFormat?.ToLowerInvariant() == "custom" &&
            settings.CustomDayMappings is { Count: > 0 })
        {
            var required = Enumerable.Range(0, 8).Select(x => x.ToString());
            var missing = required.Except(settings.CustomDayMappings.Keys);
            if (missing.Any())
                throw new ArgumentException($"CustomDayMappings is missing keys: {string.Join(", ", missing)}");

            return settings.CustomDayMappings!;
        }

        return settings.DayNameFormat?.ToLowerInvariant() switch
        {
            "full" => Full,
            "single" => Single, 
            _ => Short
        };
    }
}
