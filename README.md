[![](https://img.shields.io/nuget/v/soenneker.deduplication.bounded.registry.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.deduplication.bounded.registry/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.deduplication.bounded.registry/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.deduplication.bounded.registry/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.deduplication.bounded.registry.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.deduplication.bounded.registry/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.deduplication.bounded.registry/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.deduplication.bounded.registry/actions/workflows/codeql.yml)

# Soenneker.Deduplication.Bounded.Registry

A keyed registry of bounded dedupe instances backed by `Soenneker.Dictionaries.Singletons.SingletonDictionary{TValue,T1}`.

## Install

```bash
dotnet add package Soenneker.Deduplication.Bounded.Registry
```

## Quick start

```csharp
using Soenneker.Deduplication.Bounded.Registry.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddBoundedDedupeRegistryAsSingleton();
```

Adds `IBoundedDedupeRegistry` as a singleton service.

## What you get

- `IBoundedDedupeRegistry` — A keyed registry of bounded dedupe instances backed by `Soenneker.Dictionaries.Singletons.SingletonDictionary{TValue,T1}`.
- `BoundedDedupeRegistryRegistrar` — A keyed registry of bounded dedupe instances.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IBoundedDedupeRegistry.Get(key, maxSize, cancellationToken)` | Gets the bounded dedupe for `key`, creating and caching it with `maxSize` if missing. | The cached or newly created `IBoundedDedupe`. |
| `IBoundedDedupeRegistry.GetSync(key, maxSize, cancellationToken)` | Synchronously gets the bounded dedupe for `key`, creating and caching it with `maxSize` if missing. | The resulting bounded Dedupe. |
| `IBoundedDedupeRegistry.TryGet(key, value)` | Attempts to get a cached bounded dedupe for `key` without creating one. | true if the requested update was applied; otherwise, false. |
| `BoundedDedupeRegistryRegistrar.AddBoundedDedupeRegistryAsSingleton(services)` | Adds `IBoundedDedupeRegistry` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `BoundedDedupeRegistryRegistrar.AddBoundedDedupeRegistryAsScoped(services)` | Adds `IBoundedDedupeRegistry` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.
