namespace CronCraft.Models;
public class CronSettings
{
    public string DayNameFormat { get; set; } = "short";
    public Dictionary<string, string>? CustomDayMappings { get; set; }
}
