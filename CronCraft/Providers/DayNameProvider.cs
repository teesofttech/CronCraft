using CronCraft.Models;

namespace CronCraft.Providers;

public static class DayNameProvider
{
    private static readonly Dictionary<string, string> SingleEnglish = new()
    {
        { "0", "S" }, { "1", "M" }, { "2", "T" }, { "3", "W" },
        { "4", "T" }, { "5", "F" }, { "6", "S" }, { "7", "S" }
    };

    private static readonly Dictionary<string, string> ShortEnglish = new()
    {
        { "0", "Sun" }, { "1", "Mon" }, { "2", "Tue" }, { "3", "Wed" },
        { "4", "Thu" }, { "5", "Fri" }, { "6", "Sat" }, { "7", "Sun" }
    };

    private static readonly Dictionary<string, string> FullEnglish = new()
    {
        { "0", "Sunday" }, { "1", "Monday" }, { "2", "Tuesday" }, { "3", "Wednesday" },
        { "4", "Thursday" }, { "5", "Friday" }, { "6", "Saturday" }, { "7", "Sunday" }
    };


    private static readonly Dictionary<string, string> FullFrench = new()
    {
        { "0", "Dimanche" }, { "1", "Lundi" }, { "2", "Mardi" }, { "3", "Mercredi" },
        { "4", "Jeudi" }, { "5", "Vendredi" }, { "6", "Samedi" }, { "7", "Dimanche" }
    };

    private static readonly Dictionary<string, string> ShortFrench = new()
    {
        { "0", "Dim" }, { "1", "Lun" }, { "2", "Mar" }, { "3", "Mer" },
        { "4", "Jeu" }, { "5", "Ven" }, { "6", "Sam" }, { "7", "Dim" }
    };

    private static readonly Dictionary<string, string> SingleFrench = new()
    {
        { "0", "D" }, { "1", "L" }, { "2", "M" }, { "3", "M" },
        { "4", "J" }, { "5", "V" }, { "6", "S" }, { "7", "D" }
    };


    private static readonly Dictionary<string, string> FullSpanish = new()
    {
        { "0", "Domingo" }, { "1", "Lunes" }, { "2", "Martes" }, { "3", "Miércoles" },
        { "4", "Jueves" }, { "5", "Viernes" }, { "6", "Sábado" }, { "7", "Domingo" }
    };

    private static readonly Dictionary<string, string> ShortSpanish = new()
    {
        { "0", "Dom" }, { "1", "Lun" }, { "2", "Mar" }, { "3", "Mié" },
        { "4", "Jue" }, { "5", "Vie" }, { "6", "Sáb" }, { "7", "Dom" }
    };

    private static readonly Dictionary<string, string> SingleSpanish = new()
    {
        { "0", "D" }, { "1", "L" }, { "2", "M" }, { "3", "X" },
        { "4", "J" }, { "5", "V" }, { "6", "S" }, { "7", "D" }
    };

    public static Dictionary<string, string> GetDayMap(CronSettings settings)
    {
        var format = settings.DayNameFormat?.ToLowerInvariant() ?? "short";
        var language = settings.Language?.ToLowerInvariant() ?? "en";

        if (format == "custom" && settings.CustomDayMappings is { Count: > 0 })
        {
            var required = Enumerable.Range(0, 8).Select(i => i.ToString());
            var missing = required.Except(settings.CustomDayMappings.Keys);
            if (missing.Any())
            {
                throw new ArgumentException($"CustomDayMappings is missing keys: {string.Join(", ", missing)}");
            }
            return settings.CustomDayMappings!;
        }

        return (format, language) switch
        {
            ("single", "es") => SingleSpanish,
            ("single", "fr") => SingleFrench,
            ("short", "es") => ShortSpanish,
            ("short", "fr") => ShortFrench,
            ("full", "es") => FullSpanish,
            ("full", "fr") => FullFrench,
            ("full", _) => FullEnglish,
            ("single", _) => SingleEnglish,
            _ => ShortEnglish
        };
    }

}
