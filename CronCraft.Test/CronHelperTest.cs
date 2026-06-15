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
}
