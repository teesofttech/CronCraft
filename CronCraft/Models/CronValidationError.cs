namespace CronCraft.Models;

/// <summary>
/// Describes a cron validation or conversion failure without throwing an exception.
/// </summary>
/// <param name="Code">A stable, machine-readable value from <see cref="CronValidationErrorCodes"/>.</param>
/// <param name="Message">A human-readable explanation of the failure.</param>
/// <param name="Field">The cron or settings field that failed validation, when known.</param>
/// <param name="Value">The rejected value, when available.</param>
public sealed record CronValidationError(
    string Code,
    string Message,
    string? Field = null,
    string? Value = null);

/// <summary>
/// Stable error codes returned by CronCraft's non-throwing APIs.
/// </summary>
public static class CronValidationErrorCodes
{
    /// <summary>The cron expression reference is null.</summary>
    public const string NullExpression = "null_expression";

    /// <summary>The cron expression is empty or contains only whitespace.</summary>
    public const string EmptyExpression = "empty_expression";

    /// <summary>The expression contains an unsupported number of fields.</summary>
    public const string InvalidPartCount = "invalid_part_count";

    /// <summary>A cron field contains an invalid value.</summary>
    public const string InvalidFieldValue = "invalid_field_value";

    /// <summary>A cron range is malformed, outside its allowed bounds, or reversed.</summary>
    public const string InvalidRange = "invalid_range";

    /// <summary>The expression uses syntax that CronCraft does not support.</summary>
    public const string UnsupportedSyntax = "unsupported_syntax";

    /// <summary>The configured timezone identifier is invalid or unavailable.</summary>
    public const string InvalidTimeZone = "invalid_time_zone";

    /// <summary>A CronCraft setting is invalid.</summary>
    public const string InvalidSettings = "invalid_settings";
}
