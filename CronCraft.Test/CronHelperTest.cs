using CronCraft.Exceptions;
using CronCraft.Extensions;
using CronCraft.Models;

namespace CronCraft.Test;

[TestClass]
public sealed class CronHelperTest
{
    private CronSettings CreateCustomSettings()
    {
        return new CronSettings
        {
            Language = "en",
            DayNameFormat = "custom",
            CustomDayMappings = new Dictionary<string, string>
            {
                { "0", "Sunday" },
                { "1", "Monday" },
                { "2", "Tuesday" },
                { "3", "Wednesday" },
                { "4", "Thursday" },
                { "5", "Friday" },
                { "6", "Saturday" },
                { "7", "Sunday" }
            }
        };
    }

    [TestMethod]
    public void Test_Every5Minutes()
    {
        var settings = CreateCustomSettings();
        string result = CronHelper.ToHumanReadable("*/5 * * * *", settings);
        Assert.AreEqual("Every 5 minutes", result);
    }

    [TestMethod]
    public void Test_Every2Hours()
    {
        var settings = CreateCustomSettings();
        string result = CronHelper.ToHumanReadable("0 */2 * * *", settings);
        Assert.AreEqual("Every 2 hours", result);
    }

    [TestMethod]
    public void Test_Every2HoursOnWeekdays()
    {
        var settings = CreateCustomSettings();
        settings.DayNameFormat = "short";

        string result = CronHelper.ToHumanReadable("0 */2 * * 1,2,3,4,5", settings);
        Assert.AreEqual("Every 2 hours on Mon, Tue, Wed, Thu and Fri", result);
    }

    [TestMethod]
    public void Test_DayOfWeekRange()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 * * 1-5", settings);

        Assert.AreEqual("Every Monday through Friday at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_DayOfWeekRangeAndList()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 * * 1-5,7", settings);

        Assert.AreEqual("Every Monday through Friday and Sunday at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_HourRangeOnWeekdays()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 8-17 * * 1-5", settings);

        Assert.AreEqual(
            "Every hour from 08:00 AM to 05:00 PM on Monday through Friday",
            result);
    }

    [TestMethod]
    public void Test_HourList()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 8,12,17 * * *", settings);

        Assert.AreEqual("Every day at 08:00 AM, 12:00 PM and 05:00 PM", result);
    }

    [TestMethod]
    public void Test_MinuteRange()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0-30 9 * * *", settings);

        Assert.AreEqual("Every minute from 09:00 AM to 09:30 AM", result);
    }

    [TestMethod]
    public void Test_MinuteList()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0,15,30 9 * * *", settings);

        Assert.AreEqual("Every day at 09:00 AM, 09:15 AM and 09:30 AM", result);
    }

    [TestMethod]
    public void Test_DayOfMonthRange()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 0 1-7 * *", settings);

        Assert.AreEqual("Every day from the 1st through the 7th at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_DayOfMonthList()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 1,15,30 * *", settings);

        Assert.AreEqual("Every month on the 1st, 15th and 30th at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_EveryDayAtSpecificTime()
    {
        var settings = new CronSettings { DayNameFormat = "short" };

        string result = CronHelper.ToHumanReadable("30 14 * * *", settings);
        Assert.AreEqual("Every day at 02:30 PM", result);
    }

    [TestMethod]
    public void Test_DefaultTimeFormat_Is12Hour()
    {
        var settings = new CronSettings();

        Assert.AreEqual("hh:mm tt", settings.TimeFormat);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Test_BlankTimeFormat_UsesDefault(string timeFormat)
    {
        var settings = new CronSettings { TimeFormat = timeFormat };

        string result = CronHelper.ToHumanReadable("30 14 * * *", settings);

        Assert.AreEqual("hh:mm tt", settings.TimeFormat);
        Assert.AreEqual("Every day at 02:30 PM", result);
    }

    [DataTestMethod]
    [DataRow("0 0 * * *", "Every day at 00:00")]
    [DataRow("30 14 * * *", "Every day at 14:30")]
    [DataRow("59 23 * * *", "Every day at 23:59")]
    public void Test_CustomTimeFormat_24Hour(string cron, string expected)
    {
        var settings = new CronSettings { TimeFormat = "HH:mm" };

        string result = CronHelper.ToHumanReadable(cron, settings);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_CustomTimeFormat_12Hour()
    {
        var settings = new CronSettings { TimeFormat = "hh:mm tt" };

        string result = CronHelper.ToHumanReadable("30 14 * * *", settings);

        Assert.AreEqual("Every day at 02:30 PM", result);
    }

    [TestMethod]
    public void Test_CustomTimeFormat_OverridesLanguageDefault()
    {
        var settings = new CronSettings
        {
            Language = "de",
            TimeFormat = "hh:mm tt"
        };

        string result = CronHelper.ToHumanReadable("30 14 * * *", settings);

        Assert.AreEqual("Jeden Tag um 02:30 PM Uhr", result);
    }

    [TestMethod]
    public void Test_LanguageDefaultTimeFormat_IsPreservedWithoutOverride()
    {
        var settings = new CronSettings { Language = "de" };

        string result = CronHelper.ToHumanReadable("30 14 * * *", settings);

        Assert.AreEqual("Jeden Tag um 14:30 Uhr", result);
    }

    [TestMethod]
    public void Test_CustomTimeFormat_AppliesToTimeRanges()
    {
        var settings = CreateCustomSettings();
        settings.TimeFormat = "HH:mm";

        string result = CronHelper.ToHumanReadable("0 8-17 * * 1-5", settings);

        Assert.AreEqual(
            "Every hour from 08:00 to 17:00 on Monday through Friday",
            result);
    }

    [TestMethod]
    public void Test_CustomTimeFormat_AppliesToTimeLists()
    {
        var settings = CreateCustomSettings();
        settings.TimeFormat = "HH:mm";

        string result = CronHelper.ToHumanReadable("0 8,12,17 * * *", settings);

        Assert.AreEqual("Every day at 08:00, 12:00 and 17:00", result);
    }

    [TestMethod]
    public void Test_EveryMonAtTime()
    {
        var settings = CreateCustomSettings();
        settings.DayNameFormat = "short";

        string result = CronHelper.ToHumanReadable("15 10 * * 1", settings);
        Assert.AreEqual("Every Mon at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_EveryMondayAtTime()
    {
        var settings = CreateCustomSettings();
        settings.DayNameFormat = "full";

        string result = CronHelper.ToHumanReadable("15 10 * * 1", settings);
        Assert.AreEqual("Every Monday at 10:15 AM", result);
    }

    [DataTestMethod]
    [DataRow("full", "Every Monday at 09:00 AM")]
    [DataRow("short", "Every Mon at 09:00 AM")]
    [DataRow("single", "Every M at 09:00 AM")]
    public void Test_DayNameFormat_UsesRequestedBuiltInFormat(string format, string expected)
    {
        var settings = new CronSettings { DayNameFormat = format };

        string result = CronHelper.ToHumanReadable("0 9 * * 1", settings);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_DayNameFormat_Custom_UsesCustomDayMapping()
    {
        var settings = CreateCustomSettings();
        settings.CustomDayMappings!["1"] = "Moonday";

        string result = CronHelper.ToHumanReadable("0 9 * * 1", settings);

        Assert.AreEqual("Every Moonday at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_Every27thAtTime()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 4 27 * ?", settings);
        Assert.AreEqual("Every month on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_Every6MonthsOn27th()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 4 27 */6 ?", settings);
        Assert.AreEqual("Every 6 months on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificMonthAndDay()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 1 3 *", settings);

        Assert.AreEqual("Every year on March 1st at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificMonthAndDay_December()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 0 25 12 *", settings);

        Assert.AreEqual("Every year on December 25th at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificMonthWithWildcardDay()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 * 6 *", settings);

        Assert.AreEqual("Every day in June at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_MonthStepWithWildcardDay()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 9 * */3 *", settings);

        Assert.AreEqual("Every 3 months at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificMonthWithDayOfWeek()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "short" };

        string result = CronHelper.ToHumanReadable("0 9 * 6 1", settings);

        Assert.AreEqual("Every Mon in June at 09:00 AM", result);
    }

    [DataTestMethod]
    [DataRow("en", "Every year on March 1st at 09:00 AM")]
    [DataRow("es", "Cada año el día 1 de marzo a las 09:00 AM")]
    [DataRow("fr", "Chaque année le 1er mars à 09:00 AM")]
    [DataRow("de", "Jedes Jahr am 1. im März um 09:00 Uhr")]
    [DataRow("pt", "Todo ano no dia 1º de março às 09:00")]
    [DataRow("it", "Ogni anno il giorno 1 di marzo alle 09:00")]
    [DataRow("nl", "Elk jaar op 1e maart om 09:00")]
    [DataRow("zh", "每年三月第 1 天 09:00")]
    [DataRow("ja", "毎年3月1日 09:00に")]
    public void Test_SpecificMonthName_IsLocalized(string language, string expected)
    {
        var settings = new CronSettings { Language = language };

        string result = CronHelper.ToHumanReadable("0 9 1 3 *", settings);

        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("en", "Every Mon in June at 09:00 AM")]
    [DataRow("es", "Cada lun en junio a las 09:00 AM")]
    [DataRow("fr", "Chaque lun en juin à 09:00 AM")]
    [DataRow("de", "Jeden Mo im Juni um 09:00 Uhr")]
    [DataRow("pt", "Em seg em junho às 09:00")]
    [DataRow("it", "Ogni lun in giugno alle 09:00")]
    [DataRow("nl", "Elke ma in juni om 09:00")]
    [DataRow("zh", "六月每周一 09:00")]
    [DataRow("ja", "6月の毎週月曜 09:00に")]
    public void Test_SpecificMonthWithDayOfWeek_IsLocalized(string language, string expected)
    {
        var settings = new CronSettings
        {
            Language = language,
            DayNameFormat = "short"
        };

        string result = CronHelper.ToHumanReadable("0 9 * 6 1", settings);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Test_SpecificDayAndWeek()
    {
        var settings = CreateCustomSettings();
        settings.DayNameFormat = "full";

        string result = CronHelper.ToHumanReadable("0 23 15 * 1", settings);
        Assert.AreEqual("On 15th and Monday at 11:00 PM", result);
    }

    [TestMethod]
    public void Test_WithTimeZone()
    {
        var settings = CreateCustomSettings();

        TimeZoneInfo timeZone =
            TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

        string result = CronHelper.ToHumanReadable("0 4 * * *", settings, timeZone);
        Assert.AreEqual("Every day at 05:00 AM", result);
    }

    [TestMethod]
    public void Test_TimeZoneSetting_ConvertsFromUtc()
    {
        var settings = CreateCustomSettings();
        settings.TimeZone = "W. Central Africa Standard Time";

        string result = CronHelper.ToHumanReadable("0 4 * * *", settings);

        Assert.AreEqual("Every day at 05:00 AM", result);
    }

    [TestMethod]
    public void Test_InvalidTimeZoneSetting_ThrowsTimeZoneNotFoundException()
    {
        var settings = CreateCustomSettings();
        settings.TimeZone = "Not/A-Real-Time-Zone";

        Assert.ThrowsException<TimeZoneNotFoundException>(() =>
            CronHelper.ToHumanReadable("0 4 * * *", settings));
    }

    [TestMethod]
    public void Test_Quartz6PartExpression()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 15 10 * * ?", settings, null);
        Assert.AreEqual("Every day at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_Quartz7PartExpression()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("0 0 12 1/1 * ? *", settings, null);
        Assert.AreEqual("Every month on the 1st at 12:00 PM", result);
    }

    [TestMethod]
    public void Test_NullExpression_ThrowsArgumentException()
    {
        var settings = CreateCustomSettings();
        Assert.ThrowsException<ArgumentException>(() =>
            CronHelper.ToHumanReadable(null!, settings));
    }

    [TestMethod]
    public void Test_EmptyExpression_ThrowsArgumentException()
    {
        var settings = CreateCustomSettings();
        Assert.ThrowsException<ArgumentException>(() =>
            CronHelper.ToHumanReadable("", settings));
    }

    [TestMethod]
    public void Test_WhitespaceExpression_ThrowsArgumentException()
    {
        var settings = CreateCustomSettings();
        Assert.ThrowsException<ArgumentException>(() =>
            CronHelper.ToHumanReadable("   ", settings));
    }

    [TestMethod]
    public void Test_TooFewParts_ThrowsInvalidCronExpressionException()
    {
        var settings = CreateCustomSettings();
        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            CronHelper.ToHumanReadable("* * *", settings));

        StringAssert.Contains(ex.Message, "expected 5 parts but got 3");
    }

    [TestMethod]
    public void Test_TooManyParts_ThrowsInvalidCronExpressionException()
    {
        var settings = CreateCustomSettings();
        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            CronHelper.ToHumanReadable("* * * * * * * *", settings));

        StringAssert.Contains(ex.Message, "expected 5 parts but got 8");
    }

    [TestMethod]
    public void Test_WildcardOnlyExpression_DescribesEveryMinuteSchedule()
    {
        var settings = CreateCustomSettings();

        string result = CronHelper.ToHumanReadable("* * * * *", settings);

        Assert.AreEqual("Every minute", result);
    }

    [TestMethod]
    public void Test_MinuteStepAboveMaximum_ThrowsInvalidCronExpressionException()
    {
        var settings = CreateCustomSettings();

        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            CronHelper.ToHumanReadable("*/61 * * * *", settings));

        StringAssert.Contains(ex.Message, "invalid minute value '*/61'");
    }

    [TestMethod]
    public void Test_DayOfWeekAboveMaximum_ThrowsInvalidCronExpressionException()
    {
        var settings = CreateCustomSettings();

        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            CronHelper.ToHumanReadable("0 0 * * 8", settings));

        StringAssert.Contains(ex.Message, "invalid day-of-week value '8'");
    }

    [TestMethod]
    public void Test_InvalidCronExpressionException_ContainsOffendingExpression()
    {
        var settings = CreateCustomSettings();
        const string badCron = "bad expression";
        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            CronHelper.ToHumanReadable(badCron, settings));

        Assert.AreEqual(badCron, ex.CronExpression);
    }

    // --- Quartz special characters: L, W, # ---

    [TestMethod]
    public void Test_L_LastDayOfMonth()
    {
        var settings = new CronSettings { Language = "en" };
        string result = CronHelper.ToHumanReadable("0 0 L * ?", settings);
        Assert.AreEqual("Last day of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_LW_LastWeekdayOfMonth()
    {
        var settings = new CronSettings { Language = "en" };
        string result = CronHelper.ToHumanReadable("0 0 LW * ?", settings);
        Assert.AreEqual("Last weekday of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_W_NearestWeekday()
    {
        var settings = new CronSettings { Language = "en" };
        string result = CronHelper.ToHumanReadable("0 0 15W * ?", settings);
        Assert.AreEqual("Nearest weekday to the 15th of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_W_NearestWeekday_1st()
    {
        var settings = new CronSettings { Language = "en" };
        string result = CronHelper.ToHumanReadable("0 9 1W * ?", settings);
        Assert.AreEqual("Nearest weekday to the 1st of the month at 09:00 AM", result);
    }

    [TestMethod]
    public void Test_Hash_NthWeekdayOfMonth_FirstMonday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 0 ? * 2#1", settings);
        Assert.AreEqual("Every first Monday of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_Hash_NthWeekdayOfMonth_ThirdFriday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 0 ? * 6#3", settings);
        Assert.AreEqual("Every third Friday of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_Hash_NthWeekdayOfMonth_SecondWednesday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 10 ? * 4#2", settings);
        Assert.AreEqual("Every second Wednesday of the month at 10:00 AM", result);
    }

    [TestMethod]
    public void Test_L_LastDayOfWeek_LastMonday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 0 ? * 2L", settings);
        Assert.AreEqual("Last Monday of the month at 12:00 AM", result);
    }

    [TestMethod]
    public void Test_L_LastDayOfWeek_LastFriday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 17 ? * 6L", settings);
        Assert.AreEqual("Last Friday of the month at 05:00 PM", result);
    }

    [TestMethod]
    public void Test_Hash_QuartzSixPart_FirstTuesday()
    {
        var settings = new CronSettings { Language = "en", DayNameFormat = "full" };
        string result = CronHelper.ToHumanReadable("0 0 9 ? * 3#1", settings);
        Assert.AreEqual("Every first Tuesday of the month at 09:00 AM", result);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "Cada 5 minutos")]
    [DataRow("0 */2 * * *", "Cada 2 horas")]
    [DataRow("0 9 * * 1", "Cada lunes a las 09:00 AM")]
    public void Test_SpanishLocalization(string cron, string expected)
    {
        AssertLocalized("es", cron, "full", expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "Toutes les 5 minutes")]
    [DataRow("0 */2 * * *", "Toutes les 2 heures")]
    [DataRow("0 9 * * 1", "Chaque lundi à 09:00 AM")]
    public void Test_FrenchLocalization(string cron, string expected)
    {
        AssertLocalized("fr", cron, "full", expected);
    }

    [DataTestMethod]
    [DataRow("en", "Every Mon, Wed and Fri at 09:00 AM")]
    [DataRow("es", "Cada lun, mié y vie a las 09:00 AM")]
    [DataRow("fr", "Chaque lun, mer et ven à 09:00 AM")]
    [DataRow("de", "Jeden Mo, Mi und Fr um 09:00 Uhr")]
    [DataRow("pt", "Em seg, qua e sex às 09:00")]
    [DataRow("it", "Ogni lun, mer e ven alle 09:00")]
    [DataRow("nl", "Elke ma, wo en vr om 09:00")]
    [DataRow("zh", "每周一、周三和周五 09:00")]
    [DataRow("ja", "毎週月曜、水曜と金曜 09:00に")]
    public void Test_LocalizedDayList_UsesLanguageConjunction(string language, string expected)
    {
        AssertLocalized(language, "0 9 * * 1,3,5", "short", expected);
    }

    [DataTestMethod]
    [DataRow("en", "Every Mon through Fri at 09:00 AM")]
    [DataRow("es", "De lun a vie a las 09:00 AM")]
    [DataRow("fr", "Du lun au ven à 09:00 AM")]
    [DataRow("de", "Von Mo bis Fr um 09:00 Uhr")]
    [DataRow("pt", "De seg a sex às 09:00")]
    [DataRow("it", "Da lun a ven alle 09:00")]
    [DataRow("nl", "Van ma tot en met vr om 09:00")]
    [DataRow("zh", "每周一至周五 09:00")]
    [DataRow("ja", "毎週月曜から金曜まで 09:00に")]
    public void Test_LocalizedDayRange_UsesLanguageRangeConnector(string language, string expected)
    {
        AssertLocalized(language, "0 9 * * 1-5", "short", expected);
    }

    [DataTestMethod]
    [DataRow("en", "*/1 * * * *", "Every minute")]
    [DataRow("es", "*/1 * * * *", "Cada minuto")]
    [DataRow("fr", "*/1 * * * *", "Chaque minute")]
    [DataRow("de", "*/1 * * * *", "Jede Minute")]
    [DataRow("pt", "*/1 * * * *", "A cada minuto")]
    [DataRow("it", "*/1 * * * *", "Ogni minuto")]
    [DataRow("nl", "*/1 * * * *", "Elke minuut")]
    [DataRow("zh", "*/1 * * * *", "每分钟")]
    [DataRow("ja", "*/1 * * * *", "毎分")]
    [DataRow("en", "0 */1 * * *", "Every hour")]
    [DataRow("es", "0 */1 * * *", "Cada hora")]
    [DataRow("fr", "0 */1 * * *", "Chaque heure")]
    [DataRow("de", "0 */1 * * *", "Jede Stunde")]
    [DataRow("pt", "0 */1 * * *", "A cada hora")]
    [DataRow("it", "0 */1 * * *", "Ogni ora")]
    [DataRow("nl", "0 */1 * * *", "Elk uur")]
    [DataRow("zh", "0 */1 * * *", "每小时")]
    [DataRow("ja", "0 */1 * * *", "毎時")]
    [DataRow("en", "0 9 1 */1 *", "Every month on the 1st at 09:00 AM")]
    [DataRow("es", "0 9 1 */1 *", "Cada mes el día 1 a las 09:00 AM")]
    [DataRow("fr", "0 9 1 */1 *", "Chaque mois le 1er à 09:00 AM")]
    [DataRow("de", "0 9 1 */1 *", "Jeden Monat am 1. um 09:00 Uhr")]
    [DataRow("pt", "0 9 1 */1 *", "Todo mês no dia 1º às 09:00")]
    [DataRow("it", "0 9 1 */1 *", "Ogni mese il giorno 1 alle 09:00")]
    [DataRow("nl", "0 9 1 */1 *", "Elke maand op de 1e om 09:00")]
    [DataRow("zh", "0 9 1 */1 *", "每月第 1 天 09:00")]
    [DataRow("ja", "0 9 1 */1 *", "毎月1日 09:00に")]
    public void Test_SingularInterval_UsesLanguageSingularPhrase(
        string language,
        string cron,
        string expected)
    {
        AssertLocalized(language, cron, "short", expected);
    }

    [DataTestMethod]
    [DataRow("es", "Cada día a las 08:00 AM y 12:00 PM")]
    [DataRow("fr", "Chaque jour à 08:00 AM et 12:00 PM")]
    public void Test_LocalizedTimeList_UsesLanguageGrammar(string language, string expected)
    {
        AssertLocalized(language, "0 8,12 * * *", "short", expected);
    }

    [DataTestMethod]
    [DataRow("es", "Cada hora de 08:00 AM a 10:00 AM")]
    [DataRow("fr", "Chaque heure de 08:00 AM à 10:00 AM")]
    public void Test_LocalizedTimeRange_UsesLanguageGrammar(string language, string expected)
    {
        AssertLocalized(language, "0 8-10 * * *", "short", expected);
    }

    [DataTestMethod]
    [DataRow("es", "Cada día del 1 al 3 a las 09:00 AM")]
    [DataRow("fr", "Chaque jour du 1er au 3 à 09:00 AM")]
    public void Test_LocalizedDayOfMonthRange_UsesLanguageOrdinals(string language, string expected)
    {
        AssertLocalized(language, "0 9 1-3 * *", "short", expected);
    }

    [DataTestMethod]
    [DataRow("es", "Cada primer lunes del mes a las 12:00 AM")]
    [DataRow("fr", "Chaque premier lundi du mois à 12:00 AM")]
    public void Test_LocalizedNthWeekday_UsesLanguageOrdinalWords(string language, string expected)
    {
        AssertLocalized(language, "0 0 ? * 2#1", "full", expected);
    }

    [DataTestMethod]
    [DataRow("es", "Último día del mes a las 12:00 AM")]
    [DataRow("fr", "Dernier jour du mois à 12:00 AM")]
    public void Test_LocalizedQuartzSpecial_ContainsNoEnglishGrammar(string language, string expected)
    {
        AssertLocalized(language, "0 0 L * ?", "full", expected);
    }

    [TestMethod]
    public void Test_UnsupportedLanguage_FallsBackToEnglish()
    {
        var settings = new CronSettings
        {
            Language = "unsupported",
            DayNameFormat = "full"
        };

        string result = CronHelper.ToHumanReadable("0 9 * * 1", settings);

        Assert.AreEqual("Every Monday at 09:00 AM", result);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "Alle 5 Minuten")]
    [DataRow("0 */2 * * *", "short", "Alle 2 Stunden")]
    [DataRow("0 9 * * 1", "full", "Jeden Montag um 09:00 Uhr")]
    [DataRow("0 9 * * 1", "short", "Jeden Mo um 09:00 Uhr")]
    [DataRow("0 9 * * 1", "single", "Jeden M um 09:00 Uhr")]
    [DataRow("0 9 15 * *", "short", "Jeden Monat am 15. um 09:00 Uhr")]
    public void Test_GermanLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("de", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "A cada 5 minutos")]
    [DataRow("0 */2 * * *", "short", "A cada 2 horas")]
    [DataRow("0 9 * * 1", "full", "Em segunda-feira às 09:00")]
    [DataRow("0 9 * * 1", "short", "Em seg às 09:00")]
    [DataRow("0 9 * * 1", "single", "Em S às 09:00")]
    [DataRow("0 9 15 * *", "short", "Todo mês no dia 15º às 09:00")]
    public void Test_PortugueseLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("pt", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "Ogni 5 minuti")]
    [DataRow("0 */2 * * *", "short", "Ogni 2 ore")]
    [DataRow("0 9 * * 1", "full", "Ogni lunedì alle 09:00")]
    [DataRow("0 9 * * 1", "short", "Ogni lun alle 09:00")]
    [DataRow("0 9 * * 1", "single", "Ogni L alle 09:00")]
    [DataRow("0 9 15 * *", "short", "Ogni mese il giorno 15 alle 09:00")]
    public void Test_ItalianLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("it", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "Elke 5 minuten")]
    [DataRow("0 */2 * * *", "short", "Elke 2 uur")]
    [DataRow("0 9 * * 1", "full", "Elke maandag om 09:00")]
    [DataRow("0 9 * * 1", "short", "Elke ma om 09:00")]
    [DataRow("0 9 * * 1", "single", "Elke M om 09:00")]
    [DataRow("0 9 15 * *", "short", "Elke maand op de 15e om 09:00")]
    public void Test_DutchLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("nl", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "每 5 分钟")]
    [DataRow("0 */2 * * *", "short", "每 2 小时")]
    [DataRow("0 9 * * 1", "full", "每星期一 09:00")]
    [DataRow("0 9 * * 1", "short", "每周一 09:00")]
    [DataRow("0 9 * * 1", "single", "每一 09:00")]
    [DataRow("0 9 15 * *", "short", "每月第 15 天 09:00")]
    public void Test_ChineseLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("zh", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "5分ごと")]
    [DataRow("0 */2 * * *", "short", "2時間ごと")]
    [DataRow("0 9 * * 1", "full", "毎週月曜日 09:00に")]
    [DataRow("0 9 * * 1", "short", "毎週月曜 09:00に")]
    [DataRow("0 9 * * 1", "single", "毎週月 09:00に")]
    [DataRow("0 9 15 * *", "short", "毎月15日 09:00に")]
    public void Test_JapaneseLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("ja", cron, dayNameFormat, expected);
    }

    private static void AssertLocalized(
        string language,
        string cron,
        string dayNameFormat,
        string expected)
    {
        var settings = new CronSettings
        {
            Language = language,
            DayNameFormat = dayNameFormat
        };

        Assert.AreEqual(expected, CronHelper.ToHumanReadable(cron, settings));
    }
}
