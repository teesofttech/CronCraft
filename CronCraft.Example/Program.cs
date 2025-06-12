using CronCraft.Extensions;

string cronExpression = "0 0 * * *"; // Every day at midnight

var settings = new CronCraft.Models.CronSettings
{
    Language = "en",
    DayNameFormat = "short",
};

string humanReadable = cronExpression.ToHumanReadable(settings);
Console.WriteLine("🔁 CronCraft Expression Translator");
Console.WriteLine("-----------------------------------");
Console.WriteLine($"🧾 Cron Expression:   {cronExpression}");
Console.WriteLine($"📖 Human Readable:    {humanReadable}");
Console.WriteLine("-----------------------------------");
Console.WriteLine("Press Enter to exit...");


// With Local TimeZone
TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
humanReadable = cronExpression.ToHumanReadable(settings, timeZone);
Console.WriteLine($"📖 Human Readable (Local TZ): {humanReadable}");

Console.ReadLine();

