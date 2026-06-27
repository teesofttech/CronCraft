namespace CronCraft.Models
{
    /// <summary>
    /// Configuration options for CronCraft expression parsing and formatting.
    /// </summary>
    public class CronSettings
    {
        private string? _timeFormat;

        /// <summary>
        /// Language used for output formatting.
        /// Supported values: "en", "es", "fr", "de", "pt", "it", "nl", "zh", and "ja".
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
        /// .NET time format string used for rendered times.
        /// Default is "hh:mm tt" for backwards-compatible 12-hour output.
        /// Examples: "hh:mm tt" for 12-hour time, "HH:mm" for 24-hour time.
        /// </summary>
        public string TimeFormat
        {
            get => _timeFormat ?? "hh:mm tt";
            set => _timeFormat = value;
        }

        internal bool HasTimeFormatOverride => !string.IsNullOrWhiteSpace(_timeFormat);

        /// <summary>
        /// Custom mapping for day names when DayNameFormat is "custom".
        /// </summary>
        public Dictionary<string, string>? CustomDayMappings { get; set; }
    }
}
