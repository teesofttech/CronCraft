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

    private static readonly Dictionary<string, string> FullGerman =
        CreateDayMap("Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag");
    private static readonly Dictionary<string, string> ShortGerman =
        CreateDayMap("So", "Mo", "Di", "Mi", "Do", "Fr", "Sa");
    private static readonly Dictionary<string, string> SingleGerman =
        CreateDayMap("S", "M", "D", "M", "D", "F", "S");

    private static readonly Dictionary<string, string> FullPortuguese =
        CreateDayMap("Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado");
    private static readonly Dictionary<string, string> ShortPortuguese =
        CreateDayMap("Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb");
    private static readonly Dictionary<string, string> SinglePortuguese =
        CreateDayMap("D", "S", "T", "Q", "Q", "S", "S");

    private static readonly Dictionary<string, string> FullItalian =
        CreateDayMap("Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato");
    private static readonly Dictionary<string, string> ShortItalian =
        CreateDayMap("Dom", "Lun", "Mar", "Mer", "Gio", "Ven", "Sab");
    private static readonly Dictionary<string, string> SingleItalian =
        CreateDayMap("D", "L", "M", "M", "G", "V", "S");

    private static readonly Dictionary<string, string> FullDutch =
        CreateDayMap("Zondag", "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag");
    private static readonly Dictionary<string, string> ShortDutch =
        CreateDayMap("Zo", "Ma", "Di", "Wo", "Do", "Vr", "Za");
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
