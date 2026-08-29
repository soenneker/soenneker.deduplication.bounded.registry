using System.Threading;
using System.Threading.Tasks;
using Soenneker.Deduplication.Bounded.Abstract;
using Soenneker.Deduplication.Bounded.Registry.Abstract;
using Soenneker.Dictionaries.Singletons;
namespace Soenneker.Deduplication.Bounded.Registry;
public sealed class BoundedDedupeRegistry : IBoundedDedupeRegistry
{
    private readonly SingletonDictionary<IBoundedDedupe, int> _dictionary;
    /// <summary>
    /// Returns the configured bounded Dedupe used by the bounded dedupe registry.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="maxSize">Maximum number of values retained by the registry.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested bounded Dedupe.</returns>
    public BoundedDedupeRegistry()
    {
        _dictionary = new SingletonDictionary<IBoundedDedupe, int>();
        _dictionary.SetInitialization(static (_, maxSize) => new BoundedDedupe(maxSize));
    }
    /// <summary>
    /// Returns the configured bounded Dedupe used by the bounded dedupe registry.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="maxSize">Maximum number of entries to retain.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested bounded Dedupe.</returns>
    public ValueTask<IBoundedDedupe> Get(string key, int maxSize, CancellationToken cancellationToken = default) =>
        _dictionary.Get(key, maxSize, cancellationToken);

    /// <summary>
    /// Returns the configured bounded Dedupe used by the Bounded Dedupe Registry.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="maxSize">Maximum number of entries to retain.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested bounded Dedupe.</returns>
    public IBoundedDedupe GetSync(string key, int maxSize, CancellationToken cancellationToken = default) =>
        _dictionary.GetSync(key, maxSize, cancellationToken);

    public bool TryGet(string key, out IBoundedDedupe? value) =>
        _dictionary.TryGet(key, out value);

    /// <summary>
    /// Releases resources used by the current instance.
    /// </summary>
    public void Dispose() => _dictionary.Dispose();
    
    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask DisposeAsync() => _dictionary.DisposeAsync();
}
