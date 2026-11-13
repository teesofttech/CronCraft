using CronCraft.Exceptions;
using CronCraft.Models;
using CronCraft.Providers;
using System.Text.RegularExpressions;

namespace CronCraft.Extensions;

public static class CronHelper
{
    private static readonly Regex NamedValuePattern = new(
        "[A-Za-z]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly IReadOnlyDictionary<string, string> NamedWeekdays =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SUN"] = "0",
            ["MON"] = "1",
            ["TUE"] = "2",
            ["WED"] = "3",
            ["THU"] = "4",
            ["FRI"] = "5",
            ["SAT"] = "6"
        };

    private static readonly IReadOnlyDictionary<string, string> NamedMonths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["JAN"] = "1",
            ["FEB"] = "2",
            ["MAR"] = "3",
            ["APR"] = "4",
            ["MAY"] = "5",
            ["JUN"] = "6",
            ["JUL"] = "7",
            ["AUG"] = "8",
            ["SEP"] = "9",
            ["OCT"] = "10",
            ["NOV"] = "11",
            ["DEC"] = "12"
        };

    private static readonly IReadOnlyDictionary<string, string> CronAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["@hourly"] = "0 * * * *",
            ["@daily"] = "0 0 * * *",
            ["@weekly"] = "0 0 * * 0",
            ["@monthly"] = "0 0 1 * *",
            ["@yearly"] = "0 0 1 1 *"
        };

    /// <summary>
    /// Converts a cron expression to a human-readable format based on language and settings.
    /// </summary>
    /// <param name="cronExpression">The cron expression to convert.</param>
    /// <param name="settings">The settings controlling language, day-name format, and timezone.</param>
    /// <param name="timeZone">
    /// Optional timezone to convert UTC times into. When supplied, this overrides
    /// <see cref="CronSettings.TimeZone"/>.
    /// </param>
    /// <returns>A human-readable description of the cron schedule.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="cronExpression"/> is null or empty.</exception>
    /// <exception cref="InvalidCronExpressionException">Thrown when <paramref name="cronExpression"/> is malformed.</exception>
    /// <exception cref="TimeZoneNotFoundException">
    /// Thrown when <see cref="CronSettings.TimeZone"/> does not identify a system timezone.
    /// </exception>
    public static string ToHumanReadable(this string cronExpression, CronSettings settings, TimeZoneInfo? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            throw new ArgumentException("Cron expression must not be null or empty.", nameof(cronExpression));

        timeZone ??= ResolveTimeZone(settings.TimeZone);

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
            ["EveryMinute"] = "Every minute",
            ["EveryHour"] = "Every hour",
            ["EveryHourOn"] = "Every hour on {0}",
            ["EveryMonth"] = "Every month",
            ["AtSystemReboot"] = "At system reboot",
            ["AtTime"] = "at {0}",
            ["EveryXMinutes"] = "Every {0} minutes",
            ["EveryXHours"] = "Every {0} hours",
            ["EveryXHoursOn"] = "Every {0} hours on {1}",
            ["EveryMonthOnDay"] = "Every month on the {0}",
            ["EveryYearOnMonthDay"] = "Every year on {0} {1}",
            ["EveryYearOnDates"] = "Every year on {0}",
            ["MonthAndDay"] = "{0} {1}",
            ["EveryDayInMonth"] = "Every day in {0}",
            ["EveryXMonths"] = "Every {0} months",
            ["OnDayAndWeek"] = "On {0} and {1}",
            ["EveryDayOfWeek"] = "Every {0}",
            ["EveryDayOfWeekRange"] = "Every {0}",
            ["EveryDayOfWeekInMonth"] = "Every {0} in {1}",
            ["EveryXMonthsOnDay"] = "Every {0} months on the {1}",
            ["EveryHourFromTo"] = "Every hour from {0} to {1}",
            ["EveryMinuteFromTo"] = "Every minute from {0} to {1}",
            ["EveryDayAtTimes"] = "Every day at {0}",
            ["AtTimesOnDays"] = "At {0} on {1}",
            ["AtTimesOnDaysOfMonth"] = "At {0} on the {1} of every month",
            ["TimePatternOnDays"] = "{0} on {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} on the {1} of every month",
            ["EveryDayFromDayRange"] = "Every day from the {0}",
            ["NthWeekdayOfMonth"] = "Every {0} {1} of the month",
            ["LastDayOfWeekInMonth"] = "Last {0} of the month",
            ["LastWeekdayOfMonth"] = "Last weekday of the month",
            ["LastDayOfMonth"] = "Last day of the month",
            ["NearestWeekdayToDay"] = "Nearest weekday to the {0} of the month",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableSpanish(string cron, CronSettings settings, TimeZoneInfo? timeZone)
    {
        return BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Cada día",
            ["EveryMinute"] = "Cada minuto",
            ["EveryHour"] = "Cada hora",
            ["EveryHourOn"] = "Cada hora los {0}",
            ["EveryMonth"] = "Cada mes",
            ["AtSystemReboot"] = "Al reiniciar el sistema",
            ["AtTime"] = "a las {0}",
            ["EveryXMinutes"] = "Cada {0} minutos",
            ["EveryXHours"] = "Cada {0} horas",
            ["EveryXHoursOn"] = "Cada {0} horas los {1}",
            ["EveryMonthOnDay"] = "Cada mes el día {0}",
            ["EveryYearOnMonthDay"] = "Cada año el día {1} de {0}",
            ["EveryYearOnDates"] = "Cada año {0}",
            ["MonthAndDay"] = "el día {1} de {0}",
            ["EveryDayInMonth"] = "Cada día en {0}",
            ["EveryXMonths"] = "Cada {0} meses",
            ["OnDayAndWeek"] = "El {0} y los {1}",
            ["EveryDayOfWeek"] = "Cada {0}",
            ["EveryDayOfWeekRange"] = "De {0}",
            ["EveryDayOfWeekInMonth"] = "Cada {0} en {1}",
            ["EveryXMonthsOnDay"] = "Cada {0} meses el día {1}",
            ["EveryHourFromTo"] = "Cada hora de {0} a {1}",
            ["EveryMinuteFromTo"] = "Cada minuto de {0} a {1}",
            ["EveryDayAtTimes"] = "Cada día a las {0}",
            ["AtTimesOnDays"] = "A las {0} los {1}",
            ["AtTimesOnDaysOfMonth"] = "A las {0} los días {1} de cada mes",
            ["TimePatternOnDays"] = "{0} los {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} los días {1} de cada mes",
            ["EveryDayFromDayRange"] = "Cada día del {0}",
            ["NthWeekdayOfMonth"] = "Cada {0} {1} del mes",
            ["LastDayOfWeekInMonth"] = "Último {0} del mes",
            ["LastWeekdayOfMonth"] = "Último día laborable del mes",
            ["LastDayOfMonth"] = "Último día del mes",
            ["NearestWeekdayToDay"] = "Día laborable más cercano al {0} del mes",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableFrench(string cron, CronSettings settings, TimeZoneInfo? timeZone)
    {
        return BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Chaque jour",
            ["EveryMinute"] = "Chaque minute",
            ["EveryHour"] = "Chaque heure",
            ["EveryHourOn"] = "Chaque heure le {0}",
            ["EveryMonth"] = "Chaque mois",
            ["AtSystemReboot"] = "Au redémarrage du système",
            ["AtTime"] = "à {0}",
            ["EveryXMinutes"] = "Toutes les {0} minutes",
            ["EveryXHours"] = "Toutes les {0} heures",
            ["EveryXHoursOn"] = "Toutes les {0} heures le {1}",
            ["EveryMonthOnDay"] = "Chaque mois le {0}",
            ["EveryYearOnMonthDay"] = "Chaque année le {1} {0}",
            ["EveryYearOnDates"] = "Chaque année {0}",
            ["MonthAndDay"] = "le {1} {0}",
            ["EveryDayInMonth"] = "Chaque jour en {0}",
            ["EveryXMonths"] = "Tous les {0} mois",
            ["OnDayAndWeek"] = "Le {0} et le {1}",
            ["EveryDayOfWeek"] = "Chaque {0}",
            ["EveryDayOfWeekRange"] = "Du {0}",
            ["EveryDayOfWeekInMonth"] = "Chaque {0} en {1}",
            ["EveryXMonthsOnDay"] = "Tous les {0} mois le {1}",
            ["EveryHourFromTo"] = "Chaque heure de {0} à {1}",
            ["EveryMinuteFromTo"] = "Chaque minute de {0} à {1}",
            ["EveryDayAtTimes"] = "Chaque jour à {0}",
            ["AtTimesOnDays"] = "À {0} les {1}",
            ["AtTimesOnDaysOfMonth"] = "À {0} les {1} de chaque mois",
            ["TimePatternOnDays"] = "{0} les {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} les {1} de chaque mois",
            ["EveryDayFromDayRange"] = "Chaque jour du {0}",
            ["NthWeekdayOfMonth"] = "Chaque {0} {1} du mois",
            ["LastDayOfWeekInMonth"] = "Dernier {0} du mois",
            ["LastWeekdayOfMonth"] = "Dernier jour ouvré du mois",
            ["LastDayOfMonth"] = "Dernier jour du mois",
            ["NearestWeekdayToDay"] = "Jour ouvré le plus proche du {0} du mois",
            ["TimeFormat"] = "12"
        });
    }

    private static string ToHumanReadableGerman(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Jeden Tag",
            ["EveryMinute"] = "Jede Minute",
            ["EveryHour"] = "Jede Stunde",
            ["EveryHourOn"] = "Jede Stunde am {0}",
            ["EveryMonth"] = "Jeden Monat",
            ["AtSystemReboot"] = "Beim Systemneustart",
            ["AtTime"] = "um {0} Uhr",
            ["EveryXMinutes"] = "Alle {0} Minuten",
            ["EveryXHours"] = "Alle {0} Stunden",
            ["EveryXHoursOn"] = "Alle {0} Stunden am {1}",
            ["EveryMonthOnDay"] = "Jeden Monat am {0}",
            ["EveryYearOnMonthDay"] = "Jedes Jahr am {1} im {0}",
            ["EveryYearOnDates"] = "Jedes Jahr {0}",
            ["MonthAndDay"] = "am {1} im {0}",
            ["EveryDayInMonth"] = "Jeden Tag im {0}",
            ["EveryXMonths"] = "Alle {0} Monate",
            ["OnDayAndWeek"] = "Am {0} und am {1}",
            ["EveryDayOfWeek"] = "Jeden {0}",
            ["EveryDayOfWeekRange"] = "Von {0}",
            ["EveryDayOfWeekInMonth"] = "Jeden {0} im {1}",
            ["EveryXMonthsOnDay"] = "Alle {0} Monate am {1}",
            ["EveryHourFromTo"] = "Jede Stunde von {0} bis {1}",
            ["EveryMinuteFromTo"] = "Jede Minute von {0} bis {1}",
            ["EveryDayAtTimes"] = "Jeden Tag um {0} Uhr",
            ["AtTimesOnDays"] = "Um {0} Uhr am {1}",
            ["AtTimesOnDaysOfMonth"] = "Um {0} Uhr an den Tagen {1} jedes Monats",
            ["TimePatternOnDays"] = "{0} am {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} an den Tagen {1} jedes Monats",
            ["EveryDayFromDayRange"] = "Jeden Tag vom {0}",
            ["NthWeekdayOfMonth"] = "Jeden {0} {1} des Monats",
            ["LastDayOfWeekInMonth"] = "Letzter {0} des Monats",
            ["LastWeekdayOfMonth"] = "Letzter Werktag des Monats",
            ["LastDayOfMonth"] = "Letzter Tag des Monats",
            ["NearestWeekdayToDay"] = "Nächster Werktag zum {0} des Monats",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadablePortuguese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Todos os dias",
            ["EveryMinute"] = "A cada minuto",
            ["EveryHour"] = "A cada hora",
            ["EveryHourOn"] = "A cada hora em {0}",
            ["EveryMonth"] = "Todo mês",
            ["AtSystemReboot"] = "Na reinicialização do sistema",
            ["AtTime"] = "às {0}",
            ["EveryXMinutes"] = "A cada {0} minutos",
            ["EveryXHours"] = "A cada {0} horas",
            ["EveryXHoursOn"] = "A cada {0} horas em {1}",
            ["EveryMonthOnDay"] = "Todo mês no dia {0}",
            ["EveryYearOnMonthDay"] = "Todo ano no dia {1} de {0}",
            ["EveryYearOnDates"] = "Todo ano {0}",
            ["MonthAndDay"] = "no dia {1} de {0}",
            ["EveryDayInMonth"] = "Todos os dias em {0}",
            ["EveryXMonths"] = "A cada {0} meses",
            ["OnDayAndWeek"] = "No dia {0} e em {1}",
            ["EveryDayOfWeek"] = "Em {0}",
            ["EveryDayOfWeekRange"] = "De {0}",
            ["EveryDayOfWeekInMonth"] = "Em {0} em {1}",
            ["EveryXMonthsOnDay"] = "A cada {0} meses no dia {1}",
            ["EveryHourFromTo"] = "A cada hora das {0} às {1}",
            ["EveryMinuteFromTo"] = "A cada minuto das {0} às {1}",
            ["EveryDayAtTimes"] = "Todos os dias às {0}",
            ["AtTimesOnDays"] = "Às {0} em {1}",
            ["AtTimesOnDaysOfMonth"] = "Às {0} nos dias {1} de cada mês",
            ["TimePatternOnDays"] = "{0} em {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} nos dias {1} de cada mês",
            ["EveryDayFromDayRange"] = "Todos os dias do {0}",
            ["NthWeekdayOfMonth"] = "Toda {0} {1} do mês",
            ["LastDayOfWeekInMonth"] = "Última {0} do mês",
            ["LastWeekdayOfMonth"] = "Último dia útil do mês",
            ["LastDayOfMonth"] = "Último dia do mês",
            ["NearestWeekdayToDay"] = "Dia útil mais próximo do {0} do mês",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableItalian(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Ogni giorno",
            ["EveryMinute"] = "Ogni minuto",
            ["EveryHour"] = "Ogni ora",
            ["EveryHourOn"] = "Ogni ora di {0}",
            ["EveryMonth"] = "Ogni mese",
            ["AtSystemReboot"] = "Al riavvio del sistema",
            ["AtTime"] = "alle {0}",
            ["EveryXMinutes"] = "Ogni {0} minuti",
            ["EveryXHours"] = "Ogni {0} ore",
            ["EveryXHoursOn"] = "Ogni {0} ore di {1}",
            ["EveryMonthOnDay"] = "Ogni mese il giorno {0}",
            ["EveryYearOnMonthDay"] = "Ogni anno il giorno {1} di {0}",
            ["EveryYearOnDates"] = "Ogni anno {0}",
            ["MonthAndDay"] = "il giorno {1} di {0}",
            ["EveryDayInMonth"] = "Ogni giorno in {0}",
            ["EveryXMonths"] = "Ogni {0} mesi",
            ["OnDayAndWeek"] = "Il giorno {0} e di {1}",
            ["EveryDayOfWeek"] = "Ogni {0}",
            ["EveryDayOfWeekRange"] = "Da {0}",
            ["EveryDayOfWeekInMonth"] = "Ogni {0} in {1}",
            ["EveryXMonthsOnDay"] = "Ogni {0} mesi il giorno {1}",
            ["EveryHourFromTo"] = "Ogni ora dalle {0} alle {1}",
            ["EveryMinuteFromTo"] = "Ogni minuto dalle {0} alle {1}",
            ["EveryDayAtTimes"] = "Ogni giorno alle {0}",
            ["AtTimesOnDays"] = "Alle {0} di {1}",
            ["AtTimesOnDaysOfMonth"] = "Alle {0} nei giorni {1} di ogni mese",
            ["TimePatternOnDays"] = "{0} di {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} nei giorni {1} di ogni mese",
            ["EveryDayFromDayRange"] = "Ogni giorno dal {0}",
            ["NthWeekdayOfMonth"] = "Ogni {0} {1} del mese",
            ["LastDayOfWeekInMonth"] = "Ultimo {0} del mese",
            ["LastWeekdayOfMonth"] = "Ultimo giorno feriale del mese",
            ["LastDayOfMonth"] = "Ultimo giorno del mese",
            ["NearestWeekdayToDay"] = "Giorno feriale più vicino al {0} del mese",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableDutch(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "Elke dag",
            ["EveryMinute"] = "Elke minuut",
            ["EveryHour"] = "Elk uur",
            ["EveryHourOn"] = "Elk uur op {0}",
            ["EveryMonth"] = "Elke maand",
            ["AtSystemReboot"] = "Bij het opnieuw opstarten van het systeem",
            ["AtTime"] = "om {0}",
            ["EveryXMinutes"] = "Elke {0} minuten",
            ["EveryXHours"] = "Elke {0} uur",
            ["EveryXHoursOn"] = "Elke {0} uur op {1}",
            ["EveryMonthOnDay"] = "Elke maand op de {0}",
            ["EveryYearOnMonthDay"] = "Elk jaar op {1} {0}",
            ["EveryYearOnDates"] = "Elk jaar {0}",
            ["MonthAndDay"] = "op {1} {0}",
            ["EveryDayInMonth"] = "Elke dag in {0}",
            ["EveryXMonths"] = "Elke {0} maanden",
            ["OnDayAndWeek"] = "Op de {0} en op {1}",
            ["EveryDayOfWeek"] = "Elke {0}",
            ["EveryDayOfWeekRange"] = "Van {0}",
            ["EveryDayOfWeekInMonth"] = "Elke {0} in {1}",
            ["EveryXMonthsOnDay"] = "Elke {0} maanden op de {1}",
            ["EveryHourFromTo"] = "Elk uur van {0} tot {1}",
            ["EveryMinuteFromTo"] = "Elke minuut van {0} tot {1}",
            ["EveryDayAtTimes"] = "Elke dag om {0}",
            ["AtTimesOnDays"] = "Om {0} op {1}",
            ["AtTimesOnDaysOfMonth"] = "Om {0} op de {1} van elke maand",
            ["TimePatternOnDays"] = "{0} op {1}",
            ["TimePatternOnDaysOfMonth"] = "{0} op de {1} van elke maand",
            ["EveryDayFromDayRange"] = "Elke dag van de {0}",
            ["NthWeekdayOfMonth"] = "Elke {0} {1} van de maand",
            ["LastDayOfWeekInMonth"] = "Laatste {0} van de maand",
            ["LastWeekdayOfMonth"] = "Laatste werkdag van de maand",
            ["LastDayOfMonth"] = "Laatste dag van de maand",
            ["NearestWeekdayToDay"] = "Werkdag het dichtst bij de {0} van de maand",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableChinese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "每天",
            ["EveryMinute"] = "每分钟",
            ["EveryHour"] = "每小时",
            ["EveryHourOn"] = "{0}每小时",
            ["EveryMonth"] = "每月",
            ["AtSystemReboot"] = "系统重启时",
            ["AtTime"] = "{0}",
            ["EveryXMinutes"] = "每 {0} 分钟",
            ["EveryXHours"] = "每 {0} 小时",
            ["EveryXHoursOn"] = "每 {0} 小时（{1}）",
            ["EveryMonthOnDay"] = "每月第 {0} 天",
            ["EveryYearOnMonthDay"] = "每年{0}第 {1} 天",
            ["EveryYearOnDates"] = "每年{0}",
            ["MonthAndDay"] = "{0}第 {1} 天",
            ["EveryDayInMonth"] = "{0}每天",
            ["EveryXMonths"] = "每 {0} 个月",
            ["OnDayAndWeek"] = "每月第 {0} 天和{1}",
            ["EveryDayOfWeek"] = "每{0}",
            ["EveryDayOfWeekRange"] = "每{0}",
            ["EveryDayOfWeekInMonth"] = "{1}每{0}",
            ["EveryXMonthsOnDay"] = "每 {0} 个月的第 {1} 天",
            ["EveryHourFromTo"] = "每小时从{0}到{1}",
            ["EveryMinuteFromTo"] = "每分钟从{0}到{1}",
            ["EveryDayAtTimes"] = "每天{0}",
            ["AtTimesOnDays"] = "{1}{0}",
            ["AtTimesOnDaysOfMonth"] = "每月第{1}天的{0}",
            ["TimePatternOnDays"] = "{1}{0}",
            ["TimePatternOnDaysOfMonth"] = "每月第{1}天{0}",
            ["EveryDayFromDayRange"] = "每天从{0}",
            ["NthWeekdayOfMonth"] = "每月第{0}个{1}",
            ["LastDayOfWeekInMonth"] = "每月最后一个{0}",
            ["LastWeekdayOfMonth"] = "每月最后一个工作日",
            ["LastDayOfMonth"] = "每月最后一天",
            ["NearestWeekdayToDay"] = "每月最接近第{0}天的工作日",
            ["TimeFormat"] = "24"
        });

    private static string ToHumanReadableJapanese(string cron, CronSettings settings, TimeZoneInfo? timeZone) =>
        BuildHumanReadable(cron, settings, timeZone, new Dictionary<string, string>
        {
            ["EveryDay"] = "毎日",
            ["EveryMinute"] = "毎分",
            ["EveryHour"] = "毎時",
            ["EveryHourOn"] = "{0}に毎時",
            ["EveryMonth"] = "毎月",
            ["AtSystemReboot"] = "システム再起動時",
            ["AtTime"] = "{0}に",
            ["EveryXMinutes"] = "{0}分ごと",
            ["EveryXHours"] = "{0}時間ごと",
            ["EveryXHoursOn"] = "{1}に{0}時間ごと",
            ["EveryMonthOnDay"] = "毎月{0}日",
            ["EveryYearOnMonthDay"] = "毎年{0}{1}日",
            ["EveryYearOnDates"] = "毎年{0}",
            ["MonthAndDay"] = "{0}{1}日",
            ["EveryDayInMonth"] = "{0}は毎日",
            ["EveryXMonths"] = "{0}か月ごと",
            ["OnDayAndWeek"] = "毎月{0}日と{1}",
            ["EveryDayOfWeek"] = "毎週{0}",
            ["EveryDayOfWeekRange"] = "毎週{0}",
            ["EveryDayOfWeekInMonth"] = "{1}の毎週{0}",
            ["EveryXMonthsOnDay"] = "{0}か月ごとの{1}日",
            ["EveryHourFromTo"] = "{0}から{1}まで毎時",
            ["EveryMinuteFromTo"] = "{0}から{1}まで毎分",
            ["EveryDayAtTimes"] = "毎日{0}に",
            ["AtTimesOnDays"] = "{1}の{0}に",
            ["AtTimesOnDaysOfMonth"] = "毎月{1}日の{0}に",
            ["TimePatternOnDays"] = "{1}に{0}",
            ["TimePatternOnDaysOfMonth"] = "毎月{1}日に{0}",
            ["EveryDayFromDayRange"] = "{0}の間は毎日",
            ["NthWeekdayOfMonth"] = "毎月第{0}{1}",
            ["LastDayOfWeekInMonth"] = "毎月最後の{0}",
            ["LastWeekdayOfMonth"] = "毎月最後の平日",
            ["LastDayOfMonth"] = "毎月末日",
            ["NearestWeekdayToDay"] = "毎月{0}日に最も近い平日",
            ["TimeFormat"] = "24"
        });

    private static string BuildHumanReadable(
        string cron,
        CronSettings settings,
        TimeZoneInfo? timeZone,
        Dictionary<string, string> phrases)
    {
        var trimmedCron = cron.Trim();
        if (trimmedCron.Equals("@reboot", StringComparison.OrdinalIgnoreCase))
            return phrases["AtSystemReboot"];

        if (CronAliases.TryGetValue(trimmedCron, out var expandedCron))
            cron = expandedCron;

        var daysMap = DayNameProvider.GetDayMap(settings);
        var grammar = LocalizationGrammar.ForLanguage(settings.Language);

        string[] parts = ConvertQuartzToCronos(cron)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 5)
            throw new InvalidCronExpressionException(cron, $"expected 5 parts but got {parts.Length}");

        string minute = parts[0];
        string hour = parts[1];
        string dayOfMonth = parts[2];
        string month = NormalizeNamedValues(parts[3], NamedMonths);
        string dayOfWeek = NormalizeNamedValues(parts[4], NamedWeekdays);

        ValidateFields(cron, minute, hour, dayOfMonth, month, dayOfWeek);

        string time = FormatTime(hour, minute, timeZone, settings, phrases);

        string Phrase(string key, params object[] args) =>
            phrases.TryGetValue(key, out var value)
                ? string.Format(value, args)
                : string.Format(key, args);

        if (minute == "*" && hour == "*" && dayOfMonth == "*" && month == "*" && dayOfWeek == "*")
            return Phrase("EveryMinute");

        // --- Quartz special characters: L, W, # ---

        // day-of-week: 2#1 → "Every first Monday of the month"
        // Quartz day-of-week is 1-based (1=Sun…7=Sat); daysMap is 0-based (0=Sun…6=Sat)
        if (dayOfWeek.Contains('#'))
        {
            var (occurrence, dayName) = DescribeNthWeekday(dayOfWeek, daysMap, grammar);
            return $"{Phrase("NthWeekdayOfMonth", occurrence, dayName)} {Phrase("AtTime", time)}";
        }

        // day-of-week: 2L → "Last Monday of the month" (Quartz 1-based, convert to 0-based)
        if (dayOfWeek.EndsWith('L') && dayOfWeek != "L")
        {
            var quartzDay = int.TryParse(dayOfWeek.TrimEnd('L'), out var qd) ? qd : -1;
            var standardDay = (quartzDay - 1).ToString();
            var dayName = daysMap.TryGetValue(standardDay, out var n) ? n : $"Day {standardDay}";
            return $"{Phrase("LastDayOfWeekInMonth", dayName)} {Phrase("AtTime", time)}";
        }

        // day-of-month: LW → "Last weekday of the month"
        if (dayOfMonth.Equals("LW", StringComparison.OrdinalIgnoreCase))
            return $"{Phrase("LastWeekdayOfMonth")} {Phrase("AtTime", time)}";

        // day-of-month: L → "Last day of the month"
        if (dayOfMonth == "L")
            return $"{Phrase("LastDayOfMonth")} {Phrase("AtTime", time)}";

        // day-of-month: 15W → "Nearest weekday to the 15th"
        if (dayOfMonth.EndsWith('W'))
        {
            var baseDay = grammar.FormatOrdinal(dayOfMonth.TrimEnd('W'));
            return $"{Phrase("NearestWeekdayToDay", baseDay)} {Phrase("AtTime", time)}";
        }

        // --- Range and list patterns ---

        if (TryDescribeTimePattern(
                hour,
                minute,
                timeZone,
                settings,
                phrases,
                grammar,
                out var timePattern,
                out var isTimeList))
        {
            if (dayOfWeek != "*" && dayOfWeek != "?")
            {
                var days = JoinDays(dayOfWeek, daysMap, grammar);
                return isTimeList
                    ? Phrase("AtTimesOnDays", timePattern, days)
                    : Phrase("TimePatternOnDays", timePattern, days);
            }

            if (dayOfMonth != "*")
            {
                var days = DescribeOrdinals(dayOfMonth, grammar);
                return isTimeList
                    ? Phrase("AtTimesOnDaysOfMonth", timePattern, days)
                    : Phrase("TimePatternOnDaysOfMonth", timePattern, days);
            }

            return isTimeList ? Phrase("EveryDayAtTimes", timePattern) : timePattern;
        }

        if (dayOfMonth.Contains('-') && (dayOfWeek == "*" || dayOfWeek == "?"))
        {
            var dayRange = DescribeOrdinals(dayOfMonth, grammar);
            return $"{Phrase("EveryDayFromDayRange", dayRange)} {Phrase("AtTime", time)}";
        }

        if (minute.StartsWith("*/") && hour == "*")
        {
            var interval = minute.Replace("*/", "");
            return interval == "1" ? Phrase("EveryMinute") : Phrase("EveryXMinutes", interval);
        }

        if ((hour == "*" || hour.StartsWith("*/")) && minute == "0")
        {
            var interval = hour == "*" ? "1" : hour.Replace("*/", "");
            var hasDayConstraint = dayOfWeek != "*" && !IsAllDays(dayOfWeek);
            if (interval == "1")
            {
                return hasDayConstraint
                    ? Phrase("EveryHourOn", JoinDays(dayOfWeek, daysMap, grammar))
                    : Phrase("EveryHour");
            }

            return hasDayConstraint
                ? Phrase("EveryXHoursOn", interval, JoinDays(dayOfWeek, daysMap, grammar))
                : Phrase("EveryXHours", interval);
        }

        if ((dayOfWeek == "*" || dayOfWeek == "?") && dayOfMonth == "*")
        {
            if (month.StartsWith("*/"))
            {
                var interval = month.Replace("*/", "");
                var monthPhrase = interval == "1"
                    ? Phrase("EveryMonth")
                    : Phrase("EveryXMonths", interval);
                return $"{monthPhrase} {Phrase("AtTime", time)}";
            }

            if (TryGetMonthName(month, settings.Language, out var monthName))
                return $"{Phrase("EveryDayInMonth", monthName)} {Phrase("AtTime", time)}";

            if (TryDescribeMonths(month, settings.Language, grammar, out var months))
                return $"{Phrase("EveryDayInMonth", months)} {Phrase("AtTime", time)}";

            return Phrase("EveryDay") + " " + Phrase("AtTime", time);
        }

        if (dayOfWeek != "*" && dayOfMonth == "*")
        {
            if (TryGetMonthName(month, settings.Language, out var monthName))
                return $"{Phrase("EveryDayOfWeekInMonth", JoinDays(dayOfWeek, daysMap, grammar), monthName)} {Phrase("AtTime", time)}";

            if (TryDescribeMonths(month, settings.Language, grammar, out var months))
                return $"{Phrase("EveryDayOfWeekInMonth", JoinDays(dayOfWeek, daysMap, grammar), months)} {Phrase("AtTime", time)}";

            var dayPhrase = Phrase(
                dayOfWeek.Contains('-') && !dayOfWeek.Contains(',')
                    ? "EveryDayOfWeekRange"
                    : "EveryDayOfWeek",
                JoinDays(dayOfWeek, daysMap, grammar));
            return $"{dayPhrase} {Phrase("AtTime", time)}";
        }

        if (dayOfMonth != "*" && (dayOfWeek == "*" || dayOfWeek == "?"))
        {
            var ordinal = DescribeOrdinals(dayOfMonth, grammar);

            if (month.StartsWith("*/"))
            {
                var interval = month.Replace("*/", "");
                var monthPhrase = interval == "1"
                    ? Phrase("EveryMonthOnDay", ordinal)
                    : Phrase("EveryXMonthsOnDay", interval, ordinal);
                return $"{monthPhrase} {Phrase("AtTime", time)}";
            }

            if (TryGetMonthName(month, settings.Language, out var monthName))
                return $"{Phrase("EveryYearOnMonthDay", monthName, ordinal)} {Phrase("AtTime", time)}";

            if (TryDescribeMonthParts(month, settings.Language, grammar, out var monthParts))
            {
                var dates = monthParts
                    .Select(value => Phrase("MonthAndDay", value, ordinal))
                    .ToList();
                return $"{Phrase("EveryYearOnDates", grammar.Join(dates))} {Phrase("AtTime", time)}";
            }

            return $"{Phrase("EveryMonthOnDay", ordinal)} {Phrase("AtTime", time)}";
        }

        if (dayOfMonth != "*" && dayOfWeek != "*")
            return Phrase("OnDayAndWeek", DescribeOrdinals(dayOfMonth, grammar), JoinDays(dayOfWeek, daysMap, grammar))
                   + " " + Phrase("AtTime", time);

        return cron;
    }

    // "2#1" → "Every first Monday of the month"  (Quartz day 2 = Mon in 1-based, so subtract 1)
    // "6#3" → "Every third Friday of the month"
    private static (string Occurrence, string DayName) DescribeNthWeekday(
        string dayOfWeek,
        Dictionary<string, string> daysMap,
        LocalizationGrammar grammar)
    {
        var hashParts = dayOfWeek.Split('#');
        // Quartz uses 1-based day-of-week; daysMap is 0-based
        var standardDay = int.TryParse(hashParts[0], out var qd) ? (qd - 1).ToString() : hashParts[0];
        var dayName = daysMap.TryGetValue(standardDay, out var n) ? n : $"Day {standardDay}";
        return (grammar.FormatOccurrence(hashParts[1]), dayName);
    }

    private static string ConvertQuartzToCronos(string quartzCron)
    {
        string[] parts = quartzCron.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 6 || parts.Length == 7)
        {
            string minute = parts[1];
            string hour = parts[2];
            string day = parts[3] == "?" ? "*" : parts[3];
            string month = parts[4];
            // Preserve L/#-suffixed values; only replace a bare "?" with "*"
            string dayOfWeek = parts[5] == "?" ? "*" : parts[5];

            return $"{minute} {hour} {day} {month} {dayOfWeek}";
        }

        return quartzCron;
    }

    private static TimeZoneInfo? ResolveTimeZone(string? timeZoneId)
    {
        return string.IsNullOrWhiteSpace(timeZoneId)
            ? null
            : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    private static void ValidateFields(
        string cron,
        string minute,
        string hour,
        string dayOfMonth,
        string month,
        string dayOfWeek)
    {
        ValidateField(cron, minute, "minute", 0, 59);
        ValidateField(cron, hour, "hour", 0, 23);
        ValidateDayOfMonth(cron, dayOfMonth);
        ValidateField(cron, month, "month", 1, 12);
        ValidateDayOfWeek(cron, dayOfWeek);
    }

    private static void ValidateDayOfMonth(string cron, string expression)
    {
        if (expression is "L" or "LW" or "?")
            return;

        if (expression.EndsWith('W') &&
            int.TryParse(expression[..^1], out var nearestWeekday) &&
            nearestWeekday is >= 1 and <= 31)
        {
            return;
        }

        ValidateField(cron, expression, "day-of-month", 1, 31, allowQuestionMark: true);
    }

    private static void ValidateDayOfWeek(string cron, string expression)
    {
        if (expression == "?")
            return;

        if (expression.Contains('#'))
        {
            var parts = expression.Split('#');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out var day) && day is >= 1 and <= 7 &&
                int.TryParse(parts[1], out var occurrence) && occurrence is >= 1 and <= 5)
            {
                return;
            }

            ThrowInvalidField(cron, "day-of-week", expression);
        }

        if (expression.EndsWith('L') &&
            int.TryParse(expression[..^1], out var lastDay) &&
            lastDay is >= 1 and <= 7)
        {
            return;
        }

        ValidateField(cron, expression, "day-of-week", 0, 7, allowQuestionMark: true);
    }

    private static void ValidateField(
        string cron,
        string expression,
        string fieldName,
        int minimum,
        int maximum,
        bool allowQuestionMark = false)
    {
        foreach (var item in expression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (item == "*" || allowQuestionMark && item == "?")
                continue;

            var stepParts = item.Split('/');
            if (stepParts.Length > 2 ||
                stepParts.Length == 2 &&
                (!int.TryParse(stepParts[1], out var step) || step < 1 || step > maximum - minimum + 1))
            {
                ThrowInvalidField(cron, fieldName, expression);
            }

            var valueExpression = stepParts[0];
            if (valueExpression == "*")
                continue;

            var rangeParts = valueExpression.Split('-');
            if (rangeParts.Length == 1 && IsInRange(rangeParts[0], minimum, maximum))
                continue;

            if (rangeParts.Length == 2 &&
                IsInRange(rangeParts[0], minimum, maximum, out var start) &&
                IsInRange(rangeParts[1], minimum, maximum, out var end) &&
                start <= end)
            {
                continue;
            }

            ThrowInvalidField(cron, fieldName, expression);
        }

        if (string.IsNullOrWhiteSpace(expression))
            ThrowInvalidField(cron, fieldName, expression);
    }

    private static bool IsInRange(string value, int minimum, int maximum) =>
        IsInRange(value, minimum, maximum, out _);

    private static bool IsInRange(string value, int minimum, int maximum, out int parsed) =>
        int.TryParse(value, out parsed) && parsed >= minimum && parsed <= maximum;

    private static void ThrowInvalidField(string cron, string fieldName, string value) =>
        throw new InvalidCronExpressionException(cron, $"invalid {fieldName} value '{value}'");

    private static string FormatTime(
        string hour,
        string minute,
        TimeZoneInfo? timeZone,
        CronSettings settings,
        Dictionary<string, string> phrases)
    {
        int h = int.TryParse(hour.Replace("*/", "0"), out var hParsed) ? hParsed : 0;
        int m = int.TryParse(minute.Replace("*/", "0"), out var mParsed) ? mParsed : 0;

        return FormatTime(h, m, timeZone, settings, phrases);
    }

    private static string FormatTime(
        int hour,
        int minute,
        TimeZoneInfo? timeZone,
        CronSettings settings,
        Dictionary<string, string> phrases)
    {
        DateTime utcTime = new DateTime(2000, 1, 1, hour, minute, 0, DateTimeKind.Utc);

        DateTime localTime = timeZone != null
            ? TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone)
            : utcTime;

        var format = settings.HasTimeFormatOverride
            ? settings.TimeFormat
            : phrases.TryGetValue("TimeFormat", out var timeFormat) && timeFormat == "24"
                ? "HH:mm"
                : "hh:mm tt";
        return localTime.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string DescribeOrdinals(string expression, LocalizationGrammar grammar)
    {
        var descriptions = expression
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part =>
            {
                var bounds = part.Split('-', StringSplitOptions.TrimEntries);
                return bounds.Length == 2
                    ? grammar.FormatOrdinalRange(bounds[0], bounds[1])
                    : grammar.FormatOrdinal(part);
            })
            .ToList();

        return grammar.Join(descriptions);
    }

    private static string NormalizeNamedValues(
        string expression,
        IReadOnlyDictionary<string, string> names)
    {
        return NamedValuePattern.Replace(
            expression,
            match => names.TryGetValue(match.Value, out var value) ? value : match.Value);
    }

    private static bool TryDescribeMonths(
        string expression,
        string language,
        LocalizationGrammar grammar,
        out string description)
    {
        description = string.Empty;
        if (!TryDescribeMonthParts(expression, language, grammar, out var parts))
            return false;

        description = grammar.Join(parts);
        return true;
    }

    private static bool TryDescribeMonthParts(
        string expression,
        string language,
        LocalizationGrammar grammar,
        out List<string> descriptions)
    {
        descriptions = [];

        foreach (var part in expression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var range = part.Split('-', StringSplitOptions.TrimEntries);
            if (range.Length == 1 && TryGetMonthName(range[0], language, out var monthName))
            {
                descriptions.Add(monthName);
                continue;
            }

            if (range.Length == 2 &&
                TryGetMonthName(range[0], language, out var startMonth) &&
                TryGetMonthName(range[1], language, out var endMonth))
            {
                descriptions.Add(grammar.FormatRange(startMonth, endMonth));
                continue;
            }

            descriptions = [];
            return false;
        }

        return descriptions.Count > 0;
    }

    private static bool TryGetMonthName(string month, string language, out string monthName)
    {
        monthName = string.Empty;

        if (!int.TryParse(month, out var monthNumber) || monthNumber is < 1 or > 12)
            return false;

        var names = language.ToLowerInvariant() switch
        {
            "es" => new string[]
            {
                "enero", "febrero", "marzo", "abril", "mayo", "junio",
                "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
            },
            "fr" => new string[]
            {
                "janvier", "février", "mars", "avril", "mai", "juin",
                "juillet", "août", "septembre", "octobre", "novembre", "décembre"
            },
            "de" => new string[]
            {
                "Januar", "Februar", "März", "April", "Mai", "Juni",
                "Juli", "August", "September", "Oktober", "November", "Dezember"
            },
            "pt" => new string[]
            {
                "janeiro", "fevereiro", "março", "abril", "maio", "junho",
                "julho", "agosto", "setembro", "outubro", "novembro", "dezembro"
            },
            "it" => new string[]
            {
                "gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno",
                "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre"
            },
            "nl" => new string[]
            {
                "januari", "februari", "maart", "april", "mei", "juni",
                "juli", "augustus", "september", "oktober", "november", "december"
            },
            "zh" => new string[]
            {
                "一月", "二月", "三月", "四月", "五月", "六月",
                "七月", "八月", "九月", "十月", "十一月", "十二月"
            },
            "ja" => new string[]
            {
                "1月", "2月", "3月", "4月", "5月", "6月",
                "7月", "8月", "9月", "10月", "11月", "12月"
            },
            _ => new string[]
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            }
        };

        monthName = names[monthNumber - 1];
        return true;
    }

    private static bool TryDescribeTimePattern(
        string hour,
        string minute,
        TimeZoneInfo? timeZone,
        CronSettings settings,
        Dictionary<string, string> phrases,
        LocalizationGrammar grammar,
        out string description,
        out bool isTimeList)
    {
        description = string.Empty;
        isTimeList = false;

        string Phrase(string key, params object[] args) =>
            phrases.TryGetValue(key, out var value)
                ? string.Format(value, args)
                : string.Format(key, args);

        if (hour.Contains('-') && int.TryParse(minute, out var fixedMinute))
        {
            var bounds = hour.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length == 2 &&
                int.TryParse(bounds[0], out var startHour) &&
                int.TryParse(bounds[1], out var endHour))
            {
                description = Phrase(
                    "EveryHourFromTo",
                    FormatTime(startHour, fixedMinute, timeZone, settings, phrases),
                    FormatTime(endHour, fixedMinute, timeZone, settings, phrases));
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
                description = Phrase(
                    "EveryMinuteFromTo",
                    FormatTime(fixedHour, startMinute, timeZone, settings, phrases),
                    FormatTime(fixedHour, endMinute, timeZone, settings, phrases));
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
                select FormatTime(parsedHour, parsedMinute, timeZone, settings, phrases))
                .ToList();

            description = grammar.Join(times);
            isTimeList = true;
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

    private static string JoinDays(
        string dayOfWeek,
        Dictionary<string, string> daysMap,
        LocalizationGrammar grammar)
    {
        var days = dayOfWeek
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => DescribeDayPart(part, daysMap, grammar))
            .Distinct()
            .ToList();

        return grammar.Join(days, "days");
    }

    private static string DescribeDayPart(
        string part,
        Dictionary<string, string> daysMap,
        LocalizationGrammar grammar)
    {
        var bounds = part.Split('-', StringSplitOptions.TrimEntries);
        if (bounds.Length == 2)
        {
            return grammar.FormatRange(
                GetDayName(bounds[0], daysMap),
                GetDayName(bounds[1], daysMap));
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

}
