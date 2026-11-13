namespace CronCraft.Exceptions;

using CronCraft.Models;

/// <summary>
/// Thrown when a cron expression is malformed or cannot be parsed.
/// </summary>
public class InvalidCronExpressionException : ArgumentException
{
    /// <summary>
    /// The cron expression that caused the exception.
    /// </summary>
    public string CronExpression { get; }

    /// <summary>
    /// Structured details about the validation failure.
    /// </summary>
    public CronValidationError ValidationError { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="InvalidCronExpressionException"/>.
    /// </summary>
    /// <param name="cronExpression">The invalid cron expression.</param>
    /// <param name="reason">A description of why the expression is invalid.</param>
    public InvalidCronExpressionException(string cronExpression, string reason)
        : this(
            cronExpression,
            new CronValidationError(
                CronValidationErrorCodes.InvalidFieldValue,
                reason))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="InvalidCronExpressionException"/>
    /// with structured validation details.
    /// </summary>
    /// <param name="cronExpression">The invalid cron expression.</param>
    /// <param name="validationError">Structured details about the validation failure.</param>
    public InvalidCronExpressionException(
        string cronExpression,
        CronValidationError validationError)
        : this(cronExpression, validationError.Message, validationError)
    {
    }

    /// <summary>
    /// Initializes a new instance with a backwards-compatible exception reason
    /// and separate structured validation details.
    /// </summary>
    /// <param name="cronExpression">The invalid cron expression.</param>
    /// <param name="reason">The reason included in the exception message.</param>
    /// <param name="validationError">Structured details about the validation failure.</param>
    public InvalidCronExpressionException(
        string cronExpression,
        string reason,
        CronValidationError validationError)
        : base(
            $"Invalid cron expression '{cronExpression}': {reason}.",
            nameof(cronExpression))
    {
        CronExpression = cronExpression;
        ValidationError = validationError;
    }
}
