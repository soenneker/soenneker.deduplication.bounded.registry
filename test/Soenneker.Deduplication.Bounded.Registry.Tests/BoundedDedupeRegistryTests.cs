using System.Threading.Tasks;
using Soenneker.Deduplication.Bounded.Abstract;
using Soenneker.Deduplication.Bounded.Registry.Abstract;
using Soenneker.Tests.HostedUnit;
using AwesomeAssertions;

namespace Soenneker.Deduplication.Bounded.Registry.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class BoundedDedupeRegistryTests : HostedUnitTest
{
    private readonly IBoundedDedupeRegistry _util;

    public BoundedDedupeRegistryTests(Host host) : base(host)
    {
        _util = Resolve<IBoundedDedupeRegistry>(true);
    }

    [Test]
    public void Get_same_key_and_maxSize_returns_same_instance()
    {
        IBoundedDedupe a = _util.GetSync("scope-a", 10_000, System.Threading.CancellationToken.None);
        IBoundedDedupe b = _util.GetSync("scope-a", 10_000, System.Threading.CancellationToken.None);
        a.Should().BeSameAs(b);
        a.MaxSize.Should().Be(10_000);
    }

    [Test]
    public async Task Get_async_same_key_and_maxSize_returns_same_instance()
    {
        IBoundedDedupe a = await _util.Get("scope-b", 5_000, System.Threading.CancellationToken.None);
        IBoundedDedupe b = await _util.Get("scope-b", 5_000, System.Threading.CancellationToken.None);
        a.Should().BeSameAs(b);
        a.MaxSize.Should().Be(5_000);
    }

    [Test]
    public void Get_different_keys_returns_different_instances()
    {
        IBoundedDedupe a = _util.GetSync("scope-1", 1_000, System.Threading.CancellationToken.None);
        IBoundedDedupe b = _util.GetSync("scope-2", 1_000, System.Threading.CancellationToken.None);
        a.Should().NotBeSameAs(b);
    }

    [Test]
    public void Get_same_key_different_maxSize_returns_same_instance_first_maxSize_wins()
    {
        IBoundedDedupe a = _util.GetSync("scope-same", 1_000, System.Threading.CancellationToken.None);
        IBoundedDedupe b = _util.GetSync("scope-same", 2_000, System.Threading.CancellationToken.None);
        a.Should().BeSameAs(b);
        a.MaxSize.Should().Be(1_000);
    }

    [Test]
    public void TryGet_returns_false_when_key_not_created()
    {
        bool found = _util.TryGet("never-created", out IBoundedDedupe? value);
        found.Should().BeFalse();
        Assert.Null(value);
    }

    [Test]
    public void TryGet_returns_true_and_instance_after_Get()
    {
        IBoundedDedupe created = _util.GetSync("scope-try", 500, System.Threading.CancellationToken.None);
        bool found = _util.TryGet("scope-try", out IBoundedDedupe? value);
        found.Should().BeTrue();
        Assert.NotNull(value);
        created.Should().BeSameAs(value);
    }

    [Test]
    public void GetSync_returns_usable_dedupe()
    {
        IBoundedDedupe dedupe = _util.GetSync("scope-use", 100, System.Threading.CancellationToken.None);
        dedupe.TryMarkSeen("item-1").Should().BeTrue();
        dedupe.TryMarkSeen("item-1").Should().BeFalse();
        dedupe.Contains("item-1").Should().BeTrue();
    }

    [Test]
    public async Task Get_returns_usable_dedupe()
    {
        IBoundedDedupe dedupe = await _util.Get("scope-async-use", 100, System.Threading.CancellationToken.None);
        dedupe.TryMarkSeen("item-1").Should().BeTrue();
        dedupe.TryMarkSeen("item-1").Should().BeFalse();
    }

    [Test]
    public void Dispose_does_not_throw()
    {
        IBoundedDedupeRegistry registry = Resolve<IBoundedDedupeRegistry>(true);
        registry.GetSync("dispose-test", 1, System.Threading.CancellationToken.None);
        registry.Dispose();
    }

    [Test]
    public async Task DisposeAsync_does_not_throw()
    {
        IBoundedDedupeRegistry registry = Resolve<IBoundedDedupeRegistry>(true);
        await registry.Get("dispose-async-test", 1, System.Threading.CancellationToken.None);
        await registry.DisposeAsync();
    }
}

