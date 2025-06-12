using CronCraft.Helper;

namespace CronCraft.Test;

[TestClass]
public sealed class CronHelperTest
{
    [TestMethod]
    public void Test_Every5Minutes()
    {
        string result = CronHelper.ToHumanReadable("*/5 * * * *");
        Assert.AreEqual("Every 5 minutes", result);
    }

    [TestMethod]
    public void Test_Every2Hours()
    {
        string result = CronHelper.ToHumanReadable("0 */2 * * *");
        Assert.AreEqual("Every 2 hours", result);
    }

    [TestMethod]
    public void Test_Every2HoursOnWeekdays()
    {
        string result = CronHelper.ToHumanReadable("0 */2 * * 1,2,3,4,5");
        Assert.AreEqual("Every 2 hours on Mon, Tue, Wed, Thu and Fri", result);
    }

    [TestMethod]
    public void Test_EveryDayAtSpecificTime()
    {
        string result = CronHelper.ToHumanReadable("30 14 * * *");
        Assert.AreEqual("Every day at 02:30 PM", result);
    }

    [TestMethod]
    public void Test_EveryMondayAtTime()
    {
        string result = CronHelper.ToHumanReadable("15 10 * * 1");
        Assert.AreEqual("Every Mon at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_Every27thAtTime()
    {
        string result = CronHelper.ToHumanReadable("0 4 27 * ?");
        Assert.AreEqual("Every month on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_Every6MonthsOn27th()
    {
        string result = CronHelper.ToHumanReadable("0 4 27 */6 ?");
        Assert.AreEqual("Every 6 months on the 27th at 04:00 AM", result);
    }

    [TestMethod]
    public void Test_SpecificDayAndWeek()
    {
        string result = CronHelper.ToHumanReadable("0 23 15 * 1");
        Assert.AreEqual("On 15th and Mon at 11:00 PM", result);
    }

    [TestMethod]
    public void Test_WithTimeZone()
    {
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
        string result = CronHelper.ToHumanReadable("0 4 * * *", timeZone);
        Assert.AreEqual("Every day at 05:00 AM", result);
    }

    [TestMethod]
    public void Test_Quartz6PartExpression()
    {
        string result = CronHelper.ToHumanReadable("0 15 10 * * ?", null);
        Assert.AreEqual("Every day at 10:15 AM", result);
    }

    [TestMethod]
    public void Test_Quartz7PartExpression()
    {
        string result = CronHelper.ToHumanReadable("0 0 12 1/1 * ? *", null);
        Assert.AreEqual("Every month on the 1st at 12:00 PM", result);
    }
}
