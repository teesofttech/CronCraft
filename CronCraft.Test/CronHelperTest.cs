using CronCraft.Helper;
using CronCraft.Models;

namespace CronCraft.Test;

[TestClass]
public sealed class CronHelperTest
{
    private CronSettings settings = new CronSettings
    {
        DayNameFormat = "custom",
        CustomDayMappings = new()
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

    [TestMethod]
    public void Test_Every5Minutes()
    {
        string result = CronHelper.ToHumanReadable("*/5 * * * *", settings);
        Assert.AreEqual("Every 5 minutes", result);
    }

    [TestMethod]
    public void Test_Every2Hours()
    {
        string result = CronHelper.ToHumanReadable("0 */2 * * *", settings);
        Assert.AreEqual("Every 2 hours", result);
    }

    [TestMethod]
    public void Test_Every2HoursOnWeekdays()
    {
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
        settings.DayNameFormat = "short";
        string result = CronHelper.ToHumanReadable("15 10 * * 1", settings);
        Assert.AreEqual("Every Mon at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_EveryMondayAtTime()
    {
        settings.DayNameFormat = "full";
        string result = CronHelper.ToHumanReadable("15 10 * * 1", settings);
        Assert.AreEqual("Every Monday at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_Every27thAtTime()
    {
        string result = CronHelper.ToHumanReadable("0 4 27 * ?", settings);
        Assert.AreEqual("Every month on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_Every6MonthsOn27th()
    {
        string result = CronHelper.ToHumanReadable("0 4 27 */6 ?", settings);
        Assert.AreEqual("Every 6 months on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificDayAndWeek()
    {
        settings.DayNameFormat = "full";
        string result = CronHelper.ToHumanReadable("0 23 15 * 1", settings);
        Assert.AreEqual("On 15th and Monday at 11:00 PM", result);
    }

    [TestMethod]
    public void Test_WithTimeZone()
    {
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
        string result = CronHelper.ToHumanReadable("0 4 * * *", settings, timeZone);
        Assert.AreEqual("Every day at 05:00 AM", result);
    }

    [TestMethod]
    public void Test_Quartz6PartExpression()
    {
        string result = CronHelper.ToHumanReadable("0 15 10 * * ?", settings, null);
        Assert.AreEqual("Every day at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_Quartz7PartExpression()
    {
        string result = CronHelper.ToHumanReadable("0 0 12 1/1 * ? *", settings, null);
        Assert.AreEqual("Every month on the 1st at 12:00 PM", result);
    }
}
