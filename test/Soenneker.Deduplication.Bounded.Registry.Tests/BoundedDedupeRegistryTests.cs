using System.Threading.Tasks;
using Soenneker.Deduplication.Bounded.Abstract;
using Soenneker.Deduplication.Bounded.Registry.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Deduplication.Bounded.Registry.Tests;

[Collection("Collection")]
public sealed class BoundedDedupeRegistryTests : FixturedUnitTest
{
    private readonly IBoundedDedupeRegistry _util;

    public BoundedDedupeRegistryTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IBoundedDedupeRegistry>(true);
    }

    [Fact]
    public void Get_same_key_and_maxSize_returns_same_instance()
    {
        IBoundedDedupe a = _util.GetSync("scope-a", 10_000, TestContext.Current.CancellationToken);
        IBoundedDedupe b = _util.GetSync("scope-a", 10_000, TestContext.Current.CancellationToken);
        Assert.Same(a, b);
        Assert.Equal(10_000, a.MaxSize);
    }

    [Fact]
    public async Task Get_async_same_key_and_maxSize_returns_same_instance()
    {
        IBoundedDedupe a = await _util.Get("scope-b", 5_000, TestContext.Current.CancellationToken);
        IBoundedDedupe b = await _util.Get("scope-b", 5_000, TestContext.Current.CancellationToken);
        Assert.Same(a, b);
        Assert.Equal(5_000, a.MaxSize);
    }

    [Fact]
    public void Get_different_keys_returns_different_instances()
    {
        IBoundedDedupe a = _util.GetSync("scope-1", 1_000, TestContext.Current.CancellationToken);
        IBoundedDedupe b = _util.GetSync("scope-2", 1_000, TestContext.Current.CancellationToken);
        Assert.NotSame(a, b);
    }

    [Fact]
    public void Get_same_key_different_maxSize_returns_same_instance_first_maxSize_wins()
    {
        IBoundedDedupe a = _util.GetSync("scope-same", 1_000, TestContext.Current.CancellationToken);
        IBoundedDedupe b = _util.GetSync("scope-same", 2_000, TestContext.Current.CancellationToken);
        Assert.Same(a, b);
        Assert.Equal(1_000, a.MaxSize);
    }

    [Fact]
    public void TryGet_returns_false_when_key_not_created()
    {
        bool found = _util.TryGet("never-created", out IBoundedDedupe? value);
        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void TryGet_returns_true_and_instance_after_Get()
    {
        IBoundedDedupe created = _util.GetSync("scope-try", 500, TestContext.Current.CancellationToken);
        bool found = _util.TryGet("scope-try", out IBoundedDedupe? value);
        Assert.True(found);
        Assert.NotNull(value);
        Assert.Same(created, value);
    }

    [Fact]
    public void GetSync_returns_usable_dedupe()
    {
        IBoundedDedupe dedupe = _util.GetSync("scope-use", 100, TestContext.Current.CancellationToken);
        Assert.True(dedupe.TryMarkSeen("item-1"));
        Assert.False(dedupe.TryMarkSeen("item-1"));
        Assert.True(dedupe.Contains("item-1"));
    }

    [Fact]
    public async Task Get_returns_usable_dedupe()
    {
        IBoundedDedupe dedupe = await _util.Get("scope-async-use", 100, TestContext.Current.CancellationToken);
        Assert.True(dedupe.TryMarkSeen("item-1"));
        Assert.False(dedupe.TryMarkSeen("item-1"));
    }

    [Fact]
    public void Dispose_does_not_throw()
    {
        IBoundedDedupeRegistry registry = Resolve<IBoundedDedupeRegistry>(true);
        registry.GetSync("dispose-test", 1, TestContext.Current.CancellationToken);
        registry.Dispose();
    }

    [Fact]
    public async Task DisposeAsync_does_not_throw()
    {
        IBoundedDedupeRegistry registry = Resolve<IBoundedDedupeRegistry>(true);
        await registry.Get("dispose-async-test", 1, TestContext.Current.CancellationToken);
        await registry.DisposeAsync();
    }
}
