using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CronCraft.Extensions;
using CronCraft.Models;

BenchmarkRunner.Run<CronHelperBenchmarks>();

[MemoryDiagnoser]
[ShortRunJob]
public class CronHelperBenchmarks
{
    private CronSettings _settings = null!;
    private CronSettings _settingsWithTz = null!;
    private CronSettings _spanishSettings = null!;
    private CronSettings _frenchSettings = null!;
    private TimeZoneInfo _timeZone = null!;

    [GlobalSetup]
    public void Setup()
    {
        _settings = new CronSettings { Language = "en", DayNameFormat = "short" };

        _timeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
        _settingsWithTz = new CronSettings { Language = "en", DayNameFormat = "short" };

        _spanishSettings = new CronSettings { Language = "es", DayNameFormat = "short" };
        _frenchSettings = new CronSettings { Language = "fr", DayNameFormat = "short" };
    }

    [Benchmark(Description = "*/5 * * * *")]
    public string Simple_EveryFiveMinutes() =>
        "*/5 * * * *".ToHumanReadable(_settings);

    [Benchmark(Description = "0 */2 * * 1,2,3,4,5")]
    public string WithDayList_Weekdays() =>
        "0 */2 * * 1,2,3,4,5".ToHumanReadable(_settings);

    [Benchmark(Description = "0 4 27 */6 ?")]
    public string WithDayOfMonth() =>
        "0 4 27 */6 ?".ToHumanReadable(_settings);

    [Benchmark(Description = "0 15 10 * * ? (Quartz 6-part)")]
    public string Quartz6Part() =>
        "0 15 10 * * ?".ToHumanReadable(_settings);

    [Benchmark(Description = "0 0 12 1/1 * ? * (Quartz 7-part)")]
    public string Quartz7Part() =>
        "0 0 12 1/1 * ? *".ToHumanReadable(_settings);

    [Benchmark(Description = "30 14 * * * (with timezone)")]
    public string WithTimezone() =>
        "30 14 * * *".ToHumanReadable(_settingsWithTz, _timeZone);

    [Benchmark(Description = "0 9 * * 1 (Spanish)")]
    public string Spanish() =>
        "0 9 * * 1".ToHumanReadable(_spanishSettings);

    [Benchmark(Description = "0 9 * * 1 (French)")]
    public string French() =>
        "0 9 * * 1".ToHumanReadable(_frenchSettings);

    [Benchmark(Description = "0 23 15 * 1 (day + weekday)")]
    public string DayOfMonthAndWeekday() =>
        "0 23 15 * 1".ToHumanReadable(_settings);
}
