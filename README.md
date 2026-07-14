[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/releases/latest)
[![Build Status](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciDAL

NuciDAL is a lightweight Data Access Layer helper library for .NET.

## Features

- Generic repository interfaces with in-memory and file-backed implementations
- Strongly typed entities based on `EntityBase<TKey>` with a string-keyed shorthand
- File-backed repositories for JSON, XML, and CSV storage
- Consistent exception model for common data operations
- Explicit persistence via `SaveChanges()` on file repositories
- `Try*` variants for all mutating and lookup operations to avoid exception-based control flow
- Entities are cloned on store and retrieval, preventing unintended mutation of internal state

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciDAL)

### .NET CLI

```bash
dotnet add package NuciDAL
```

### Package Manager Console

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
User firstAdult = users.GetFirst(user => user.Age >= 18);
User maybeTeen = users.TryGetFirst(user => user.Age < 18);

bool exists = users.ContainsId("u2");
int total = users.EntitiesCount;
```

### 3. Use a file-backed repository (JSON example)

```csharp
using NuciDAL.Repositories;

IFileRepository<User> users = new JsonRepository<User>("users.json");

users.TryAdd(new User { Id = "u3", Name = "Carol", Age = 28 });
users.TryUpdate(new User { Id = "u3", Name = "Caroline", Age = 29 });

users.SaveChanges();
```

You can replace `JsonRepository<T>` with `XmlRepository<T>` or `CsvRepository<T>` without changing any other repository usage. Ensure the target file exists and is provisioned correctly before the repository loads it.

## Repository API

`IRepository<TKey, TDataObject>` exposes:

- Read: `Get(id)`, `TryGet(id)`, `GetFirst(predicate)`, `TryGetFirst(predicate)`, `GetRandom()`, `GetAll()`, `ContainsId(id)`, `EntitiesCount`
- Write: `Add(entity)`, `TryAdd(entity)`, `Update(entity)`, `TryUpdate(entity)`, `Remove(id|entity)`, `TryRemove(id|entity)`

`IFileRepository<TKey, TDataObject>` extends this with `SaveChanges()`.

## Exception Model

The throwing variants use explicit exceptions:

| Exception | Thrown when |
|-----------|-------------|
| `EntityAlreadyExistsException` | `Add` is called with an id that already exists |
| `EntityNotFoundException` | `Get`, `GetFirst`, `Update`, or `Remove` cannot find the requested entity |
| `DuplicateEntityException` | Duplicate ids are encountered while loading file data |

## Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build NuciDAL
```

### Test

```bash
dotnet test NuciDAL.slnx
```

### Pack

```bash
dotnet pack NuciDAL -c Release
```

## Project Structure

The solution contains the following projects:

- `NuciDAL`: The main library
- `NuciDAL.UnitTests`: Unit tests

Key directories inside `NuciDAL/`:

| Directory | Purpose |
|-----------|---------|
| `DataObjects/` | Base entity classes (`EntityBase`, `EntityBase<TKey>`) |
| `IO/` | Low-level file helpers for JSON, XML, CSV, and Windows-1252 encodings |
| `Repositories/` | Repository interfaces and concrete implementations |

### Dependencies

| Package | Purpose |
|---------|---------|
| `NuciExtensions` | Entity cloning and collection utilities |

## Contributing

Contributions are welcome.

Please:

- keep the changes cross-platform
- keep the public APIs intact, unless the change is intentionally breaking
- keep the pull requests focused and consistent with the existing style
- update the documentation when the behaviour changes
- add or update the tests for any new behaviour

## Support

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or giving a ⭐️ on GitHub!

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.