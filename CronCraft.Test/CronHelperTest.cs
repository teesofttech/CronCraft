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
    [DataRow("es", "Cada año el día 1st de Marzo a las 09:00 AM")]
    [DataRow("fr", "Chaque année le 1st en Mars à 09:00 AM")]
    [DataRow("de", "Jedes Jahr am 1. im März um 09:00 Uhr")]
    [DataRow("pt", "Todo ano no dia 1º de Março às 09:00")]
    [DataRow("it", "Ogni anno il giorno 1 di Marzo alle 09:00")]
    [DataRow("nl", "Elk jaar op 1e Maart om 09:00")]
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
    [DataRow("es", "Cada Lun en Junio a las 09:00 AM")]
    [DataRow("fr", "Chaque Lun en Juin à 09:00 AM")]
    [DataRow("de", "Jeden Mo im Juni um 09:00 Uhr")]
    [DataRow("pt", "Em Seg em Junho às 09:00")]
    [DataRow("it", "Ogni Lun in Giugno alle 09:00")]
    [DataRow("nl", "Elke Ma in Juni om 09:00")]
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
    [DataRow("0 9 * * 1", "full", "Em Segunda-feira às 09:00")]
    [DataRow("0 9 * * 1", "short", "Em Seg às 09:00")]
    [DataRow("0 9 * * 1", "single", "Em S às 09:00")]
    [DataRow("0 9 15 * *", "short", "Todo mês no dia 15º às 09:00")]
    public void Test_PortugueseLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("pt", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "Ogni 5 minuti")]
    [DataRow("0 */2 * * *", "short", "Ogni 2 ore")]
    [DataRow("0 9 * * 1", "full", "Ogni Lunedì alle 09:00")]
    [DataRow("0 9 * * 1", "short", "Ogni Lun alle 09:00")]
    [DataRow("0 9 * * 1", "single", "Ogni L alle 09:00")]
    [DataRow("0 9 15 * *", "short", "Ogni mese il giorno 15 alle 09:00")]
    public void Test_ItalianLocalization(string cron, string dayNameFormat, string expected)
    {
        AssertLocalized("it", cron, dayNameFormat, expected);
    }

    [DataTestMethod]
    [DataRow("*/5 * * * *", "short", "Elke 5 minuten")]
    [DataRow("0 */2 * * *", "short", "Elke 2 uur")]
    [DataRow("0 9 * * 1", "full", "Elke Maandag om 09:00")]
    [DataRow("0 9 * * 1", "short", "Elke Ma om 09:00")]
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
