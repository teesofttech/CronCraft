using CronCraft.Helper;
using CronCraft.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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