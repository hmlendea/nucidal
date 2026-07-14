[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/releases/latest)
[![Build Status](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciDAL

NuciDAL is a lightweight Data Access Layer helper library for .NET.

It provides generic repository abstractions and ready-to-use implementations for:
- In-memory storage
- JSON file storage
- XML file storage
- CSV file storage

## Why NuciDAL

- Simple, generic repository API
- Strongly typed entities based on `EntityBase<TKey>`
- Consistent exception model for common data operations
- File-backed repositories with explicit `SaveChanges()`
- In-memory and file-based repositories share the same contract

## Requirements

- .NET SDK/runtime with support for `net10.0`

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciDAL)

### .NET CLI

```bash
dotnet add package NuciDAL
```

### NuGet Package Manager

```powershell
Install-Package NuciDAL
```

## Quick Start

### 1. Define an entity

```csharp
using NuciDAL.DataObjects;

public class User : EntityBase
{
		public string Name { get; set; }
		public int Age { get; set; }
}
```

### 2. Use an in-memory repository

```csharp
using NuciDAL.Repositories;

IRepository<User> users = new Repository<User>();

users.Add(new User { Id = "u1", Name = "Alice", Age = 31 });
users.Add(new User { Id = "u2", Name = "Bob", Age = 24 });

User byId = users.Get("u1");
User firstAdult = users.GetFirst(x => x.Age >= 18);
User maybeTeen = users.TryGetFirst(x => x.Age < 18);

bool exists = users.ContainsId("u2");
int total = users.EntitiesCount;
```

### 3. Use a file-backed repository (JSON example)

```csharp
using NuciDAL.Repositories;

IFileRepository<User> users = new JsonRepository<User>("users.json");

users.TryAdd(new User { Id = "u3", Name = "Carol", Age = 28 });
users.TryUpdate(new User { Id = "u3", Name = "Caroline", Age = 29 });

// Persist changes to disk
users.SaveChanges();
```

You can switch `JsonRepository<T>` with `XmlRepository<T>` or `CsvRepository<T>` without changing the high-level repository usage.

## Repository API Overview

Main operations exposed by `IRepository<TKey, TDataObject>`:

- Read:
	- `Get(id)` / `TryGet(id)`
	- `GetFirst(predicate)` / `TryGetFirst(predicate)`
	- `GetRandom()`
	- `GetAll()`
	- `ContainsId(id)`
	- `EntitiesCount`
- Write:
	- `Add(entity)` / `TryAdd(entity)`
	- `Update(entity)` / `TryUpdate(entity)`
	- `Remove(id|entity)` / `TryRemove(id|entity)`

`IFileRepository<TKey, TDataObject>` extends this with:
- `SaveChanges()`

## Exception Behavior

The throwing methods use explicit exceptions for invalid operations:

- `EntityAlreadyExistsException` when adding a duplicate id with `Add`
- `EntityNotFoundException` when requested data is missing in `Get`, `GetFirst`, `Update`, or `Remove`
- `DuplicateEntityException` when duplicate ids are encountered while loading file data

Use the `Try*` variants when you prefer non-throwing behavior.

## Notes

- Entities are cloned internally when stored/retrieved, so callers do not mutate the internal repository state by reference.
- File-based repositories load data lazily and persist explicitly through `SaveChanges()`.
- For file repositories, ensure the target files are provisioned according to your application setup before loading.

## Development

### Build

```bash
dotnet build
```

### Pack

```bash
dotnet pack -c Release
```

## Contributing

Contributions are welcome.

Please:

- keep the changes cross-platform
- keep the public APIs intact, unless the change is intentionally breaking
- keep the pull requests focused and consistent with the existing style
- update the documentation when the behaviour changes
- add or update the tests for any new behaviour

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.