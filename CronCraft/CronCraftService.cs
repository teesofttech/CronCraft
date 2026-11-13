using CronCraft.Extensions;
using CronCraft.Models;
using Microsoft.Extensions.Options;

namespace CronCraft;

public class CronCraftService
{
    private readonly CronSettings _settings;

    public CronCraftService(IOptions<CronSettings> settings)
    {
        _settings = settings.Value;
    }

    public string Convert(string cronExpression, TimeZoneInfo? timeZone = null)
    {
        return cronExpression.ToHumanReadable(_settings, timeZone);
    }

    /// <summary>
    /// Attempts to convert an expression without throwing for invalid input.
    /// </summary>
    /// <param name="cronExpression">The expression to convert.</param>
    /// <param name="description">The rendered description when conversion succeeds.</param>
    /// <param name="error">Structured failure details when conversion fails.</param>
    /// <param name="timeZone">Optional timezone override.</param>
    /// <returns><see langword="true"/> when conversion succeeds.</returns>
    public bool TryConvert(
        string? cronExpression,
        out string? description,
        out CronValidationError? error,
        TimeZoneInfo? timeZone = null)
    {
        return cronExpression.TryToHumanReadable(
            _settings,
            out description,
            out error,
            timeZone);
    }

    /// <summary>
    /// Attempts to validate an expression and returns structured failure details.
    /// </summary>
    /// <param name="cronExpression">The expression to validate.</param>
    /// <param name="error">Structured failure details when validation fails.</param>
    /// <param name="timeZone">Optional timezone override.</param>
    /// <returns><see langword="true"/> when the expression is valid.</returns>
    public bool TryValidate(
        string? cronExpression,
        out CronValidationError? error,
        TimeZoneInfo? timeZone = null)
    {
        return cronExpression.TryValidate(out error, _settings, timeZone);
    }

    /// <summary>
    /// Returns whether an expression can be converted with this service's settings.
    /// </summary>
    /// <param name="cronExpression">The expression to validate.</param>
    /// <param name="timeZone">Optional timezone override.</param>
    /// <returns><see langword="true"/> when the expression is valid.</returns>
    public bool IsValid(string? cronExpression, TimeZoneInfo? timeZone = null)
    {
        return cronExpression.IsValid(_settings, timeZone);
    }
}
