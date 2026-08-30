[![](https://img.shields.io/nuget/v/soenneker.deduplication.bounded.registry.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.deduplication.bounded.registry/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.deduplication.bounded.registry/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.deduplication.bounded.registry/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.deduplication.bounded.registry.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.deduplication.bounded.registry/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.deduplication.bounded.registry/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.deduplication.bounded.registry/actions/workflows/codeql.yml)

# Soenneker.Deduplication.Bounded.Registry

A thread-safe registry that creates and reuses one bounded in-memory deduplicator per string key.

## Installation

```bash
dotnet add package Soenneker.Deduplication.Bounded.Registry
```

## Registration

```csharp
using Soenneker.Deduplication.Bounded.Registry.Registrars;

services.AddBoundedDedupeRegistryAsSingleton();
```

Use the singleton registration when keys should share dedupe history across requests. The scoped registration creates a separate registry and separate history for each dependency-injection scope:

```csharp
services.AddBoundedDedupeRegistryAsScoped();
```

## Usage

```csharp
using Soenneker.Deduplication.Bounded.Abstract;
using Soenneker.Deduplication.Bounded.Registry.Abstract;

public sealed class EventConsumer(IBoundedDedupeRegistry registry)
{
    public async ValueTask<bool> ShouldProcess(string tenantId, string eventId, CancellationToken cancellationToken)
    {
        IBoundedDedupe dedupe = await registry.Get(
            key: $"tenant:{tenantId}",
            maxSize: 100_000,
            cancellationToken);

        return dedupe.TryMarkSeen(eventId);
    }
}
```

The first successful `Get` or `GetSync` for a key creates its instance. Later calls with the same key return that instance, even when they supply a different `maxSize`; choose the size consistently at the call site.

## Managing registry keys

Each distinct registry key owns another bounded set. The sets are bounded, but the number of registry keys is not, so do not use arbitrary user input as a key without controlling its cardinality.

Remove keys that are no longer active:

```csharp
await registry.Remove($"tenant:{tenantId}", cancellationToken);

// Or discard all cached histories:
await registry.Clear(cancellationToken);
```

After removal, a later `Get` creates an empty deduplicator with the newly supplied size. `TryGet` performs a lookup without creating anything.

Disposing the registry releases its underlying cache. Do not use a registry or a deduplicator obtained from it after the registry has been disposed.
