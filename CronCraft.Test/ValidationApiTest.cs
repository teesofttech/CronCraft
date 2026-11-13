using CronCraft.Exceptions;
using CronCraft.Extensions;
using CronCraft.Models;
using Microsoft.Extensions.Options;

namespace CronCraft.Test;

[TestClass]
public sealed class ValidationApiTest
{
    [TestMethod]
    public void TryToHumanReadable_ValidExpression_MatchesThrowingApi()
    {
        const string cron = "0 9 * * MON-FRI";
        var settings = new CronSettings { DayNameFormat = "full" };
        string expected = cron.ToHumanReadable(settings);

        bool success = cron.TryToHumanReadable(
            settings,
            out var description,
            out var error);

        Assert.IsTrue(success);
        Assert.AreEqual(expected, description);
        Assert.IsNull(error);
    }

    [TestMethod]
    public void TryToHumanReadable_NullExpression_ReturnsStructuredError()
    {
        string? cron = null;

        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out var description,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNull(description);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.NullExpression, error.Code);
        Assert.AreEqual("expression", error.Field);
        Assert.IsNull(error.Value);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void TryToHumanReadable_EmptyExpression_ReturnsStructuredError(string cron)
    {
        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out var description,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNull(description);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.EmptyExpression, error.Code);
        Assert.AreEqual("expression", error.Field);
        Assert.AreEqual(cron, error.Value);
    }

    [TestMethod]
    public void TryToHumanReadable_WrongPartCount_ReturnsStructuredError()
    {
        const string cron = "* * *";

        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out _,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.InvalidPartCount, error.Code);
        Assert.AreEqual("expression", error.Field);
        Assert.AreEqual(cron, error.Value);
    }

    [TestMethod]
    public void TryToHumanReadable_InvalidField_ReturnsFieldAndValue()
    {
        const string cron = "*/61 * * * *";

        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out _,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.InvalidFieldValue, error.Code);
        Assert.AreEqual("minute", error.Field);
        Assert.AreEqual("*/61", error.Value);
    }

    [TestMethod]
    public void TryToHumanReadable_InvalidRange_IsDistinguishable()
    {
        const string cron = "0 0 * * 5-1";

        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out _,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.InvalidRange, error.Code);
        Assert.AreEqual("day-of-week", error.Field);
        Assert.AreEqual("5-1", error.Value);
    }

    [TestMethod]
    public void TryToHumanReadable_InvalidTimeZone_ReturnsStructuredError()
    {
        var settings = new CronSettings { TimeZone = "Not/A-Real-Time-Zone" };

        bool success = "0 9 * * *".TryToHumanReadable(
            settings,
            out _,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.InvalidTimeZone, error.Code);
        Assert.AreEqual(nameof(CronSettings.TimeZone), error.Field);
        Assert.AreEqual(settings.TimeZone, error.Value);
    }

    [TestMethod]
    public void TryToHumanReadable_UnsupportedSyntax_IsDistinguishable()
    {
        const string cron = "@sometimes";

        bool success = cron.TryToHumanReadable(
            new CronSettings(),
            out _,
            out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.UnsupportedSyntax, error.Code);
        Assert.AreEqual("expression", error.Field);
        Assert.AreEqual(cron, error.Value);
    }

    [TestMethod]
    public void TryValidate_ReturnsErrorWithoutThrowing()
    {
        bool success = "0 0 * * 8".TryValidate(out var error);

        Assert.IsFalse(success);
        Assert.IsNotNull(error);
        Assert.AreEqual(CronValidationErrorCodes.InvalidFieldValue, error.Code);
        Assert.AreEqual("day-of-week", error.Field);
    }

    [DataTestMethod]
    [DataRow("0 9 * * 1", true)]
    [DataRow("@daily", true)]
    [DataRow("0 0 * * 8", false)]
    [DataRow("", false)]
    public void IsValid_ReturnsExpectedResult(string cron, bool expected)
    {
        Assert.AreEqual(expected, cron.IsValid());
        Assert.AreEqual(expected, CronHelper.IsValid(cron));
    }

    [TestMethod]
    public void ThrowingApi_PreservesBehaviorAndExposesStructuredDetails()
    {
        const string cron = "0 0 * * 8";

        var ex = Assert.ThrowsException<InvalidCronExpressionException>(() =>
            cron.ToHumanReadable(new CronSettings()));

        StringAssert.Contains(ex.Message, "invalid day-of-week value '8'");
        Assert.AreEqual(CronValidationErrorCodes.InvalidFieldValue, ex.ValidationError.Code);
        Assert.AreEqual("day-of-week", ex.ValidationError.Field);
        Assert.AreEqual("8", ex.ValidationError.Value);
    }

    [TestMethod]
    public void CronCraftService_ExposesNonThrowingApis()
    {
        var service = new CronCraftService(Options.Create(new CronSettings()));

        bool converted = service.TryConvert(
            "@daily",
            out var description,
            out var conversionError);
        bool validated = service.TryValidate("0 0 * * 8", out var validationError);

        Assert.IsTrue(converted);
        Assert.AreEqual("Every day at 12:00 AM", description);
        Assert.IsNull(conversionError);
        Assert.IsFalse(validated);
        Assert.IsNotNull(validationError);
        Assert.AreEqual(CronValidationErrorCodes.InvalidFieldValue, validationError.Code);
        Assert.IsTrue(service.IsValid("0 9 * * 1"));
        Assert.IsFalse(service.IsValid("0 0 * * 8"));
    }
}
