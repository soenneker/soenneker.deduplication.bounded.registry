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
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="maxSize">Maximum number of entries to retain.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The resulting bounded Dedupe.</returns>
    IBoundedDedupe GetSync(string key, int maxSize, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get a cached bounded dedupe for <paramref name="key"/> without creating one.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="value">Receives the matching value when the lookup succeeds.</param>
    /// <returns>true if the requested update was applied; otherwise, false.</returns>
    bool TryGet(string key, out IBoundedDedupe? value);

    /// <summary>
    /// Removes the dedupe instance for <paramref name="key"/> so a later lookup creates a new one.
    /// </summary>
    /// <param name="key">Registry key to remove.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><c>true</c> when an instance was removed; otherwise <c>false</c>.</returns>
    ValueTask<bool> Remove(string key, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronously removes the dedupe instance for <paramref name="key"/>.
    /// </summary>
    /// <param name="key">Registry key to remove.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><c>true</c> when an instance was removed; otherwise <c>false</c>.</returns>
    bool RemoveSync(string key, System.Threading.CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes every cached dedupe instance.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    ValueTask Clear(System.Threading.CancellationToken cancellationToken = default);
}
