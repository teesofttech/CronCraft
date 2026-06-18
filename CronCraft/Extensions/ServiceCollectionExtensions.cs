using CronCraft.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CronCraft.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CronCraft and its configuration options to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add CronCraft to.</param>
    /// <param name="configure">
    /// An optional delegate used to configure <see cref="CronSettings"/>.
    /// </param>
    /// <returns>The service collection so that additional calls can be chained.</returns>
    public static IServiceCollection AddCronCraft(
        this IServiceCollection services,
        Action<CronSettings>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<CronSettings>();

        if (configure is not null)
            services.Configure(configure);

        services.AddSingleton<CronCraftService>();

        return services;
    }
}
