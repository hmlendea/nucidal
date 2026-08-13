[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/releases/latest)
[![Build Status](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciDAL

NuciDAL is a lightweight Data Access Layer helper library for .NET. It provides generic repository interfaces with both in-memory and file-backed implementations (JSON, XML, CSV), enabling seamless data persistence whilst maintaining type safety and a consistent API across storage backends. Ideal for applications requiring flexible data access patterns without the complexity of full-featured ORMs.

## 📑 Table of Contents

- [Capabilities](#-capabilities)
- [Installation](#-installation)
- [Usage](#-usage)
- [Exception Model](#exception-model)
- [Development](#-development)
- [Project Structure](#-project-structure)
- [Contributing](#-contributing)
- [Supporting the Project](#-supporting-the-project)
- [License](#-license)

## ✨ Capabilities

- Generic repository interfaces with in-memory and file-backed implementations
- Strongly typed entities based on `EntityBase<TKey>` with a string-keyed shorthand
- File-backed repositories for JSON, XML, and CSV storage
- Consistent exception model for common data operations
- Explicit persistence via `SaveChanges()` on file repositories
- `Try*` variants for all mutating and lookup operations to avoid exception-based control flow
- Entities are cloned on store and retrieval, preventing unintended mutation of internal state
- Flexible querying with `Find(predicate)` supporting LINQ composition and lazy evaluation

## 📦 Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciDAL)

### CLI Installation

```bash
dotnet add package NuciDAL
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciDAL
```

## 🚀 Usage

### Quick Start

#### 1. Define an Entity

```csharp
using NuciDAL.DataObjects;

public class User : EntityBase
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

#### 2. In-Memory Repository

```csharp
using NuciDAL.Repositories;

IRepository<User> users = new Repository<User>();

users.Add(new User { Id = "u1", Name = "Alice", Age = 31 });
users.Add(new User { Id = "u2", Name = "Bob", Age = 24 });
users.Add(new User { Id = "u3", Name = "Carol", Age = 17 });

// Lookup operations
User byId = users.Get("u1");
User firstAdult = users.GetFirst(user => user.Age >= 18);
User maybeTeen = users.TryGetFirst(user => user.Age < 18);

// Query with Find and LINQ
IEnumerable<User> adults = users.Find(user => user.Age >= 18);
var adultNames = users
    .Find(user => user.Age >= 18)
    .Select(user => user.Name)
    .OrderBy(name => name)
    .ToList();

// Other operations
bool exists = users.ContainsId("u2");
int total = users.EntitiesCount;
```

#### 3. File-Backed Repository

```csharp
using NuciDAL.Repositories;

IFileRepository<User> users = new JsonRepository<User>("users.json");

users.TryAdd(new User { Id = "u3", Name = "Carol", Age = 28 });
users.TryUpdate(new User { Id = "u3", Name = "Caroline", Age = 29 });

users.SaveChanges();
```

You can replace `JsonRepository<T>` with `XmlRepository<T>` or `CsvRepository<T>` without changing any other repository usage. Ensure the target file exists and is provisioned correctly before the repository loads it.

### API Documentation

#### Main Interfaces

**`IRepository<TKey, TDataObject>`** – Core repository interface for in-memory entity storage and retrieval.

**`IFileRepository<TKey, TDataObject>`** – Extends `IRepository<TKey, TDataObject>` with file persistence (`SaveChanges()`).

#### Repository Methods

`IRepository<TKey, TDataObject>` exposes:

- **Read:** `Get(id)`, `TryGet(id)`, `GetFirst(predicate)`, `TryGetFirst(predicate)`, `GetRandom()`, `GetAll()`, `Find(predicate)`, `ContainsId(id)`, `EntitiesCount`
- **Write:** `Add(entity)`, `TryAdd(entity)`, `Update(entity)`, `TryUpdate(entity)`, `Remove(id|entity)`, `TryRemove(id|entity)`

`IFileRepository<TKey, TDataObject>` extends this with `SaveChanges()`.

#### Usage Examples

The `Find(predicate)` method returns `IEnumerable<T>` with lazy evaluation, allowing you to compose queries using standard LINQ operators without materialising results until enumeration:

```csharp
// Find all active users (lazy evaluation)
IEnumerable<User> activeUsers = users.Find(u => u.IsActive);

// Combine with LINQ operators
var result = users
    .Find(u => u.Age >= 18)
    .Where(u => u.Name.Contains("A"))
    .OrderBy(u => u.Name)
    .Take(10)
    .ToList();

// Use aggregation
int count = users.Find(u => u.Age > 21).Count();
decimal avgAge = users.Find(u => u.IsActive).Average(u => u.Age);

// Chain multiple operations
var names = users
    .Find(u => true)
    .Select(u => u.Name)
    .Distinct()
    .OrderBy(n => n)
    .ToList();
```

The predicate is used to create a snapshot of matching entities at the time `Find()` is called. Subsequent LINQ operations are evaluated lazily upon enumeration.

### Exception Model

The throwing variants use explicit exceptions:

| Exception | Thrown when |
|-----------|-------------|
| `EntityAlreadyExistsException` | `Add` is called with an id that already exists |
| `EntityNotFoundException` | `Get`, `GetFirst`, `Update`, or `Remove` cannot find the requested entity |
| `DuplicateEntityException` | Duplicate ids are encountered while loading file data |

## 🛠️ Development

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

### Release

```bash
dotnet pack NuciDAL -c Release
```

## 🗂️ Project Structure

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

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:

- Maintain cross-platform compatibility
- Maintain the existing public contract intact unless a breaking change is intentional
- Maintain the pull requests as focused and consistent with the existing code style
- Maintain your branch up-to-date with `master`
- Revise the documentation when the behaviour changes
- Properly test all changes, including edge cases and error conditions
- Add unit tests for any new or changed functionality

## 💝 Supporting the Project

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucidal/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

## 🔒 License

Licensed under the GNU General Public License v3.0 or later. See [LICENSE](./LICENSE) for details.