using CronCraft.Extensions;
using CronCraft.Models;
using Microsoft.Extensions.Options;

namespace CronCraft;
public class CronCraftService
{
    private readonly CronSettings _settings;

    public CronCraftService(IOptions<CronSettings> settings)
    {
        _settings = settings.Value;
    }

    public string Convert(string cronExpression, TimeZoneInfo timeZone = null)
    {
        return cronExpression.ToHumanReadable(_settings, timeZone);
    }
}