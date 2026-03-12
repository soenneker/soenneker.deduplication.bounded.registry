using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Deduplication.Bounded.Registry.Abstract;

namespace Soenneker.Deduplication.Bounded.Registry.Registrars;

/// <summary>
/// A keyed registry of bounded dedupe instances.
/// </summary>
public static class BoundedDedupeRegistryRegistrar
{
    /// <summary>
    /// Adds <see cref="IBoundedDedupeRegistry"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddBoundedDedupeRegistryAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IBoundedDedupeRegistry, BoundedDedupeRegistry>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IBoundedDedupeRegistry"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddBoundedDedupeRegistryAsScoped(this IServiceCollection services)
    {
        services.TryAddScoped<IBoundedDedupeRegistry, BoundedDedupeRegistry>();

        return services;
    }
}
