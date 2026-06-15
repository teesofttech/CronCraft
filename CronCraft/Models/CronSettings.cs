namespace CronCraft.Models
{
    /// <summary>
    /// Configuration options for CronCraft expression parsing and formatting.
    /// </summary>
    public class CronSettings
    {
        /// <summary>
        /// Language used for output formatting (e.g., "en", "es", "fr").
        /// Default is "en".
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Format used for day names in output.
        /// Allowed values: "full", "short", "single", "custom".
        /// Default is "short".
        /// </summary>
        public string DayNameFormat { get; set; } = "short";

        /// <summary>
        /// Optional timezone identifier (IANA or Windows).
        /// If null, UTC is used.
        /// Example: "W. Europe Standard Time".
        /// </summary>
        public string? TimeZone { get; set; }

        /// <summary>
        /// Custom mapping for day names when DayNameFormat is "custom".
        /// </summary>
        public Dictionary<string, string>? CustomDayMappings { get; set; }
    }
}
