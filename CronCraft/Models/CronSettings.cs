namespace CronCraft.Models;
public class CronSettings
{
    public string Language { get; set; } = "en"; // e.g., en, es, fr
    public string DayNameFormat { get; set; } = "short";
    public Dictionary<string, string>? CustomDayMappings { get; set; }
}
