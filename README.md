[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/releases/latest)
[![Build Status](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciDAL

NuciDAL is a lightweight data access layer helper library for .NET. It provides generic repository abstractions with both in-memory and file-backed implementations, so applications can persist strongly typed entities through a consistent API without adopting a full ORM.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
  - [Quick Start](#quick-start)
    - [1. Define an Entity](#1-define-an-entity)
    - [2. Use an In-Memory Repository](#2-use-an-in-memory-repository)
    - [3. Use a File-Backed Repository](#3-use-a-file-backed-repository)
- [Installation](#installation)
  - [CLI Installation](#cli-installation)
- [Development](#development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Run](#run)
  - [Test](#test)
  - [Release](#release)
  - [Dependencies](#dependencies)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [Supporting the Project](#supporting-the-project)
- [License](#license)

## ✨ Capabilities

- Generic repository interfaces for keyed entities and string-keyed entities
- In-memory and file-backed repositories with JSON, XML, and CSV persistence
- Explicit persistence for file repositories via `SaveChanges()`
- `Try*` variants for lookup and mutation operations when exception-driven flow is undesirable
- Predicate-based querying through `Find(...)`, with standard LINQ composition
- Entity cloning on storage and retrieval to reduce unintended mutation of repository state
- A compact exception model for duplicate and missing entities

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

#### 2. Use an In-Memory Repository

```csharp
using System.Collections.Generic;

using NuciDAL.Repositories;

IRepository<User> users = new Repository<User>();

users.Add(new User { Id = "u1", Name = "Alice", Age = 31 });
users.Add(new User { Id = "u2", Name = "Bob", Age = 24 });

User byId = users.Get("u1");
User firstAdult = users.GetFirst(user => user.Age >= 18);
IEnumerable<User> adults = users.Find(user => user.Age >= 18);
bool exists = users.ContainsId("u2");
int total = users.EntitiesCount;
```

#### 3. Use a File-Backed Repository

```csharp
using NuciDAL.Repositories;

IFileRepository<User> users = new JsonRepository<User>("users.json");

users.TryAdd(new User { Id = "u3", Name = "Carol", Age = 28 });
users.TryUpdate(new User { Id = "u3", Name = "Caroline", Age = 29 });

users.SaveChanges();
```

You can replace `JsonRepository<T>` with `XmlRepository<T>` or `CsvRepository<T>` without changing the surrounding repository usage.

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciDAL)

### CLI Installation

```bash
dotnet add package NuciDAL
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciDAL
```

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build NuciDAL/NuciDAL.csproj
```

### Run

NuciDAL is a library and does not include a standalone executable. Reference it from a consuming project to exercise it at runtime.

### Test

```bash
dotnet test NuciDAL.slnx
```

### Release

```bash
dotnet pack NuciDAL/NuciDAL.csproj -c Release
```

### Dependencies

| Package | Purpose |
|---------|---------|
| `NuciExtensions` | Entity cloning and collection utility extensions used by the core library |

## 🗂️ Project Structure

The solution contains the subsequent projects:
- `NuciDAL`: Core library with entity bases, repository abstractions, and file-backed persistence implementations
- `NuciDAL.UnitTests`: NUnit test project covering repository and entity behaviour

The key directories inside `NuciDAL/` are:
| Directory | Purpose |
|-----------|---------|
| `DataObjects/` | Base entity types, including generic and string-keyed entity foundations |
| `IO/` | File helpers for CSV, JSON, XML, and Windows-1252 encoded content |
| `Repositories/` | Repository interfaces, in-memory implementation, file-backed repositories, and domain exceptions |

## 🏗️ Architecture

See [ARCHITECTURE.md](./ARCHITECTURE.md) for a structural synopsis and component interactions.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Maintain the existing public contract intact unless a breaking change is intentional
- Maintain the pull requests as focused and consistent with the existing code style
- Maintain your branch up-to-date with `master`
- Revise the documentation when behaviour changes
- Properly test all changes, including edge cases and error conditions
- Add unit tests for any new or changed functionality

## 💝 Supporting the Project

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucidal/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3 or later`.
See [LICENSE](./LICENSE) for further information.