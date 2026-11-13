namespace CronCraft.Providers;

internal sealed class LocalizationGrammar
{
    private static readonly LocalizationGrammar English =
        new(" and ", ", ", "{0} through {1}", "{0} through the {1}", EnglishOrdinal, EnglishOccurrence);
    private static readonly LocalizationGrammar Spanish =
        new(" y ", ", ", "{0} a {1}", "{0} al {1}", n => n.ToString(), n => n switch
        {
            1 => "primer",
            2 => "segundo",
            3 => "tercer",
            4 => "cuarto",
            5 => "quinto",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar French =
        new(" et ", ", ", "{0} au {1}", "{0} au {1}", n => n == 1 ? "1er" : n.ToString(), n => n switch
        {
            1 => "premier",
            2 => "deuxième",
            3 => "troisième",
            4 => "quatrième",
            5 => "cinquième",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar German =
        new(" und ", ", ", "{0} bis {1}", "{0} bis zum {1}", n => $"{n}.", n => n switch
        {
            1 => "ersten",
            2 => "zweiten",
            3 => "dritten",
            4 => "vierten",
            5 => "fünften",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar Portuguese =
        new(" e ", ", ", "{0} a {1}", "{0} ao {1}", n => $"{n}º", n => n switch
        {
            1 => "primeira",
            2 => "segunda",
            3 => "terceira",
            4 => "quarta",
            5 => "quinta",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar Italian =
        new(" e ", ", ", "{0} a {1}", "{0} al {1}", n => n.ToString(), n => n switch
        {
            1 => "primo",
            2 => "secondo",
            3 => "terzo",
            4 => "quarto",
            5 => "quinto",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar Dutch =
        new(" en ", ", ", "{0} tot en met {1}", "{0} tot en met de {1}", n => $"{n}e", n => n switch
        {
            1 => "eerste",
            2 => "tweede",
            3 => "derde",
            4 => "vierde",
            5 => "vijfde",
            _ => n.ToString()
        });
    private static readonly LocalizationGrammar Chinese =
        new("和", "、", "{0}至{1}", "{0}至{1}", n => n.ToString(), n => n.ToString());
    private static readonly LocalizationGrammar Japanese =
        new("と", "、", "{0}から{1}まで", "{0}から{1}まで", n => n.ToString(), n => n.ToString());

    private readonly Func<int, string> _ordinalFormatter;
    private readonly Func<int, string> _occurrenceFormatter;

    private LocalizationGrammar(
        string conjunctionSeparator,
        string listSeparator,
        string rangeFormat,
        string ordinalRangeFormat,
        Func<int, string> ordinalFormatter,
        Func<int, string> occurrenceFormatter)
    {
        ConjunctionSeparator = conjunctionSeparator;
        ListSeparator = listSeparator;
        RangeFormat = rangeFormat;
        OrdinalRangeFormat = ordinalRangeFormat;
        _ordinalFormatter = ordinalFormatter;
        _occurrenceFormatter = occurrenceFormatter;
    }

    private string ConjunctionSeparator { get; }

    private string ListSeparator { get; }

    private string RangeFormat { get; }

    private string OrdinalRangeFormat { get; }

    public static LocalizationGrammar ForLanguage(string? language)
    {
        return language?.ToLowerInvariant() switch
        {
            "es" => Spanish,
            "fr" => French,
            "de" => German,
            "pt" => Portuguese,
            "it" => Italian,
            "nl" => Dutch,
            "zh" => Chinese,
            "ja" => Japanese,
            _ => English
        };
    }

    public string FormatOrdinal(string value)
    {
        var number = value.Contains('/') ? value.Split('/')[0] : value;
        return int.TryParse(number, out var parsed) ? _ordinalFormatter(parsed) : value;
    }

    public string FormatRange(string start, string end) =>
        string.Format(RangeFormat, start, end);

    public string FormatOrdinalRange(string start, string end) =>
        string.Format(OrdinalRangeFormat, FormatOrdinal(start), FormatOrdinal(end));

    public string FormatOccurrence(string value) =>
        int.TryParse(value, out var occurrence) ? _occurrenceFormatter(occurrence) : value;

    public string Join(IReadOnlyList<string> descriptions, string fallback = "")
    {
        return descriptions.Count switch
        {
            > 2 => string.Join(ListSeparator, descriptions.Take(descriptions.Count - 1))
                   + ConjunctionSeparator + descriptions[^1],
            2 => descriptions[0] + ConjunctionSeparator + descriptions[1],
            1 => descriptions[0],
            _ => fallback,
        };
    }

    private static string EnglishOrdinal(int number)
    {
        return number switch
        {
            int _ when number % 100 is >= 11 and <= 13 => $"{number}th",
            int _ when number % 10 == 1 => $"{number}st",
            int _ when number % 10 == 2 => $"{number}nd",
            int _ when number % 10 == 3 => $"{number}rd",
            _ => $"{number}th",
        };
    }

    private static string EnglishOccurrence(int number)
    {
        return number switch
        {
            1 => "first",
            2 => "second",
            3 => "third",
            4 => "fourth",
            5 => "fifth",
            _ => EnglishOrdinal(number)
        };
    }
}
