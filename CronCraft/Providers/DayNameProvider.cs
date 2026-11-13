using CronCraft.Models;

namespace CronCraft.Providers;

public static class DayNameProvider
{
    private static Dictionary<string, string> CreateDayMap(params string[] names) => new()
    {
        { "0", names[0] }, { "1", names[1] }, { "2", names[2] }, { "3", names[3] },
        { "4", names[4] }, { "5", names[5] }, { "6", names[6] }, { "7", names[0] }
    };

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
        { "0", "dimanche" }, { "1", "lundi" }, { "2", "mardi" }, { "3", "mercredi" },
        { "4", "jeudi" }, { "5", "vendredi" }, { "6", "samedi" }, { "7", "dimanche" }
    };

    private static readonly Dictionary<string, string> ShortFrench = new()
    {
        { "0", "dim" }, { "1", "lun" }, { "2", "mar" }, { "3", "mer" },
        { "4", "jeu" }, { "5", "ven" }, { "6", "sam" }, { "7", "dim" }
    };

    private static readonly Dictionary<string, string> SingleFrench = new()
    {
        { "0", "D" }, { "1", "L" }, { "2", "M" }, { "3", "M" },
        { "4", "J" }, { "5", "V" }, { "6", "S" }, { "7", "D" }
    };


    private static readonly Dictionary<string, string> FullSpanish = new()
    {
        { "0", "domingo" }, { "1", "lunes" }, { "2", "martes" }, { "3", "miércoles" },
        { "4", "jueves" }, { "5", "viernes" }, { "6", "sábado" }, { "7", "domingo" }
    };

    private static readonly Dictionary<string, string> ShortSpanish = new()
    {
        { "0", "dom" }, { "1", "lun" }, { "2", "mar" }, { "3", "mié" },
        { "4", "jue" }, { "5", "vie" }, { "6", "sáb" }, { "7", "dom" }
    };

    private static readonly Dictionary<string, string> SingleSpanish = new()
    {
        { "0", "D" }, { "1", "L" }, { "2", "M" }, { "3", "X" },
        { "4", "J" }, { "5", "V" }, { "6", "S" }, { "7", "D" }
    };

    private static readonly Dictionary<string, string> FullGerman =
        CreateDayMap("Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag");
    private static readonly Dictionary<string, string> ShortGerman =
        CreateDayMap("So", "Mo", "Di", "Mi", "Do", "Fr", "Sa");
    private static readonly Dictionary<string, string> SingleGerman =
        CreateDayMap("S", "M", "D", "M", "D", "F", "S");

    private static readonly Dictionary<string, string> FullPortuguese =
        CreateDayMap("domingo", "segunda-feira", "terça-feira", "quarta-feira", "quinta-feira", "sexta-feira", "sábado");
    private static readonly Dictionary<string, string> ShortPortuguese =
        CreateDayMap("dom", "seg", "ter", "qua", "qui", "sex", "sáb");
    private static readonly Dictionary<string, string> SinglePortuguese =
        CreateDayMap("D", "S", "T", "Q", "Q", "S", "S");

    private static readonly Dictionary<string, string> FullItalian =
        CreateDayMap("domenica", "lunedì", "martedì", "mercoledì", "giovedì", "venerdì", "sabato");
    private static readonly Dictionary<string, string> ShortItalian =
        CreateDayMap("dom", "lun", "mar", "mer", "gio", "ven", "sab");
    private static readonly Dictionary<string, string> SingleItalian =
        CreateDayMap("D", "L", "M", "M", "G", "V", "S");

    private static readonly Dictionary<string, string> FullDutch =
        CreateDayMap("zondag", "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag");
    private static readonly Dictionary<string, string> ShortDutch =
        CreateDayMap("zo", "ma", "di", "wo", "do", "vr", "za");
    private static readonly Dictionary<string, string> SingleDutch =
        CreateDayMap("Z", "M", "D", "W", "D", "V", "Z");

    private static readonly Dictionary<string, string> FullChinese =
        CreateDayMap("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");
    private static readonly Dictionary<string, string> ShortChinese =
        CreateDayMap("周日", "周一", "周二", "周三", "周四", "周五", "周六");
    private static readonly Dictionary<string, string> SingleChinese =
        CreateDayMap("日", "一", "二", "三", "四", "五", "六");

    private static readonly Dictionary<string, string> FullJapanese =
        CreateDayMap("日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日");
    private static readonly Dictionary<string, string> ShortJapanese =
        CreateDayMap("日曜", "月曜", "火曜", "水曜", "木曜", "金曜", "土曜");
    private static readonly Dictionary<string, string> SingleJapanese =
        CreateDayMap("日", "月", "火", "水", "木", "金", "土");

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
            ("single", "de") => SingleGerman,
            ("single", "pt") => SinglePortuguese,
            ("single", "it") => SingleItalian,
            ("single", "nl") => SingleDutch,
            ("single", "zh") => SingleChinese,
            ("single", "ja") => SingleJapanese,
            ("short", "es") => ShortSpanish,
            ("short", "fr") => ShortFrench,
            ("short", "de") => ShortGerman,
            ("short", "pt") => ShortPortuguese,
            ("short", "it") => ShortItalian,
            ("short", "nl") => ShortDutch,
            ("short", "zh") => ShortChinese,
            ("short", "ja") => ShortJapanese,
            ("full", "es") => FullSpanish,
            ("full", "fr") => FullFrench,
            ("full", "de") => FullGerman,
            ("full", "pt") => FullPortuguese,
            ("full", "it") => FullItalian,
            ("full", "nl") => FullDutch,
            ("full", "zh") => FullChinese,
            ("full", "ja") => FullJapanese,
            ("full", _) => FullEnglish,
            ("single", _) => SingleEnglish,
            _ => ShortEnglish
        };
    }

}
