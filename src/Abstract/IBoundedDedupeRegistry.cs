using System;
using System.Threading.Tasks;
using Soenneker.Deduplication.Bounded.Abstract;

namespace Soenneker.Deduplication.Bounded.Registry.Abstract;

/// <summary>
/// A keyed registry of bounded dedupe instances backed by <see cref="Soenneker.Dictionaries.Singletons.SingletonDictionary{TValue,T1}"/>.
/// </summary>
public interface IBoundedDedupeRegistry : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the bounded dedupe for <paramref name="key"/>, creating and caching it with <paramref name="maxSize"/> if missing.
    /// </summary>
    /// <param name="key">Registry key (e.g. scope or stream name).</param>
    /// <param name="maxSize">Maximum size of the dedupe set; used when the instance is created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created <see cref="IBoundedDedupe"/>.</returns>
    ValueTask<IBoundedDedupe> Get(string key, int maxSize, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously gets the bounded dedupe for <paramref name="key"/>, creating and caching it with <paramref name="maxSize"/> if missing.
    /// </summary>
    IBoundedDedupe GetSync(string key, int maxSize, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get a cached bounded dedupe for <paramref name="key"/> without creating one.
    /// </summary>
    bool TryGet(string key, out IBoundedDedupe? value);
}
