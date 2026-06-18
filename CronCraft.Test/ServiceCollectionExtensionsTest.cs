using CronCraft.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CronCraft.Test;

[TestClass]
public sealed class ServiceCollectionExtensionsTest
{
    [TestMethod]
    public void AddCronCraft_RegistersServiceAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddCronCraft();

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<CronCraftService>();
        var second = provider.GetRequiredService<CronCraftService>();

        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void AddCronCraft_AppliesConfiguration()
    {
        var services = new ServiceCollection();
        services.AddCronCraft(settings =>
        {
            settings.Language = "en";
            settings.DayNameFormat = "full";
        });

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<Models.CronSettings>>();
        var cronCraft = provider.GetRequiredService<CronCraftService>();

        Assert.AreEqual("en", options.Value.Language);
        Assert.AreEqual("full", options.Value.DayNameFormat);
        Assert.AreEqual("Every Monday at 09:00 AM", cronCraft.Convert("0 9 * * 1"));
    }

    [TestMethod]
    public void AddCronCraft_ReturnsServiceCollectionForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddCronCraft();

        Assert.AreSame(services, result);
    }
}
