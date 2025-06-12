// See https://aka.ms/new-console-template for more information
using CronCraft.Extensions;

string cronExpression = "0 0 * * *"; // Every day at midnight

var settings = new CronCraft.Models.CronSettings
{
    Language = "fr",
    DayNameFormat = "short",

};

string humanReadable = cronExpression.ToHumanReadable(settings);
Console.WriteLine($"Cron Expression: {cronExpression}");
Console.WriteLine($"Human Readable: {humanReadable}");

Console.ReadLine();

