namespace CronCraft.Exceptions;

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
    /// Initializes a new instance of <see cref="InvalidCronExpressionException"/>.
    /// </summary>
    /// <param name="cronExpression">The invalid cron expression.</param>
    /// <param name="reason">A description of why the expression is invalid.</param>
    public InvalidCronExpressionException(string cronExpression, string reason)
        : base($"Invalid cron expression '{cronExpression}': {reason}.", nameof(cronExpression))
    {
        CronExpression = cronExpression;
    }
}
