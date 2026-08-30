using System.Threading;
using System.Threading.Tasks;
using Soenneker.Deduplication.Bounded.Abstract;
using Soenneker.Deduplication.Bounded.Registry.Abstract;
using Soenneker.Dictionaries.Singletons;
namespace Soenneker.Deduplication.Bounded.Registry;
public sealed class BoundedDedupeRegistry : IBoundedDedupeRegistry
{
    private readonly SingletonDictionary<IBoundedDedupe, int> _dictionary;
    public BoundedDedupeRegistry()
    {
        _dictionary = new SingletonDictionary<IBoundedDedupe, int>();
        _dictionary.SetInitialization(static (_, maxSize) => new BoundedDedupe(maxSize));
    }
    public ValueTask<IBoundedDedupe> Get(string key, int maxSize, CancellationToken cancellationToken = default) =>
        _dictionary.Get(key, maxSize, cancellationToken);

    public IBoundedDedupe GetSync(string key, int maxSize, CancellationToken cancellationToken = default) =>
        _dictionary.GetSync(key, maxSize, cancellationToken);

    public bool TryGet(string key, out IBoundedDedupe? value) =>
        _dictionary.TryGet(key, out value);

    public ValueTask<bool> Remove(string key, CancellationToken cancellationToken = default) =>
        _dictionary.Remove(key, cancellationToken);

    public bool RemoveSync(string key, CancellationToken cancellationToken = default) =>
        _dictionary.RemoveSync(key, cancellationToken);

    public ValueTask Clear(CancellationToken cancellationToken = default) =>
        _dictionary.Clear(cancellationToken);

    public void Dispose() => _dictionary.Dispose();
    
    public ValueTask DisposeAsync() => _dictionary.DisposeAsync();
}
