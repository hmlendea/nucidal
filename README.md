[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/releases/latest)
[![Build Status](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucidal/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciDAL)](https://nuget.org/packages/NuciDAL)
[![License](https://img.shields.io/github/license/hmlendea/nucidal)](https://github.com/hmlendea/nucidal/blob/master/LICENSE)

# NuciDAL

NuciDAL is a lightweight data access layer helper library for .NET. It provides generic repository abstractions with both in-memory and file-backed implementations, so applications can persist strongly typed entities through a consistent API without adopting a full ORM.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
  - [Quick Start](#quick-start)
    - [1. Define an Entity](#1-define-an-entity)
    - [2. Use an In-Memory Repository](#2-use-an-in-memory-repository)
    - [3. Use a File-Backed Repository](#3-use-a-file-backed-repository)
    - [4. Register Repositories](#4-register-repositories)
- [Installation](#installation)
  - [Package Manager Installation](#package-manager-installation)
- [Extensibility](#extensibility)
- [Privacy and Data](#privacy-and-data)
- [Development](#development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Run](#run)
  - [Test](#test)
  - [Release](#release)
  - [Dependencies](#dependencies)
- [GitHub Actions](#github-actions)
- [Project Structure](#project-structure)
  - [Projects and Packages](#projects-and-packages)
  - [Directories](#directories)
- [Architecture](#architecture)
- [Documentation](#documentation)
- [Data Formats and Protocols](#data-formats-and-protocols)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [Project Engagement](#project-engagement)
- [Security](#security)
- [License](#license)

## ✨ Capabilities

- Generic repository interfaces for keyed entities and string-keyed entities
- In-memory and file-backed repositories with JSON, XML, and CSV persistence
- Dependency injection registrations for singleton in-memory, JSON, XML, and CSV repositories
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

#### 4. Register Repositories

```csharp
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.DependencyInjection;

IServiceCollection services = new ServiceCollection();
string usersStorePath = "users.json";

services.AddRepository<User>();
services.AddJsonRepository<User>(() => usersStorePath);
```

`AddRepository<TDataObject>` exposes `IRepository<TDataObject>`. The file-backed registrations expose `IFileRepository<TDataObject>`; select the format required by the application:

Use `services.AddXmlRepository<User>(() => "users.xml")` for XML, or `services.AddCsvRepository<User>(() => "users.csv")` for CSV instead of the JSON registration.

Every registration uses singleton lifetime. File store-path functions are evaluated when their repositories are first resolved, rather than when services are registered.

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciDAL)

### Package Manager Installation

```bash
dotnet add package NuciDAL
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciDAL
```

## 🧱 Extensibility

NuciDAL exposes the following public extension points:

| Extension Point | Contract | Purpose |
|-----------------|----------|---------|
| Entity and Key Types | `EntityBase<TKey>` / `EntityBase` | Define custom entity types with any key type |
| Repository Implementations | `IRepository<TKey, TDataObject>` / `IFileRepository<TKey, TDataObject>` | Implement custom repository backends |
| File Formats | `FileRepository<TKey, TDataObject>` | Add new serialisation formats by implementing `LoadCore` and `SaveCore` |
| Repository Registrations | `RepositoryServiceCollectionExtensions` | Register custom repository implementations in DI |

## 🛡️ Privacy and Data

NuciDAL is a .NET library providing generic repository abstractions and file-backed persistence (JSON, XML, CSV). The library itself does not collect, process, transmit, or store personal data. All data handling occurs in the consuming application, which is outside the scope of this document.

For the detailed description of how the application handles privacy and personal data, see the [Privacy Statement](./PRIVACY.md).

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

The repository includes `release.sh`, which delegates to the upstream deployment script used by the project maintainer.

```bash
bash ./release.sh 4.1.0
```

This script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.0.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

### Dependencies

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 10.0.14 | Runtime | Dependency injection contracts used by repository registrations |
| `NuciExtensions` | 6.0.2 | Runtime | Entity cloning and collection utility extensions used by the core library |
| `coverlet.collector` | 10.1.0 | Development | Code coverage collection for tests |
| `Microsoft.Extensions.DependencyInjection` | 10.0.14 | Development | DI container for test scenarios |
| `Microsoft.NET.Test.Sdk` | 18.11.0 | Development | Test platform SDK |
| `NUnit` | 5.0.0 | Development | Unit testing framework |
| `NUnit3TestAdapter` | 6.3.1 | Development | NUnit test adapter for test platform |

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [ dotnet.yml ](https://github.com/hmlendea/nucidal/blob/master/.github/workflows/dotnet.yml) | Continuous integration | Restores, builds, and tests the solution on every push and pull request to master |
| [ github-release.yml ](https://github.com/hmlendea/nucidal/blob/master/.github/workflows/github-release.yml) | GitHub Release | Creates a GitHub Release with packaged artefacts when a version tag is pushed |
| [ nuget-release.yml ](https://github.com/hmlendea/nucidal/blob/master/.github/workflows/nuget-release.yml) | NuGet Release | Publishes the NuGet package to NuGet.org when a version tag is pushed |

## 🗂️ Project Structure

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `NuciDAL/NuciDAL.csproj` | Library | Core library with entity bases, repository abstractions, and file-backed persistence implementations |
| `NuciDAL.UnitTests/NuciDAL.UnitTests.csproj` | Test | NUnit test project covering repository and entity behaviour |

### Directories

| Directory | Purpose |
|-----------|---------|
| `NuciDAL/DataObjects/` | Base entity types, including generic and string-keyed entity foundations |
| `NuciDAL/DependencyInjection/` | Service collection registrations for repository implementations |
| `NuciDAL/IO/` | File helpers for CSV, JSON, XML, and Windows-1252 encoded content |
| `NuciDAL/Repositories/` | Repository interfaces, in-memory implementation, file-backed repositories, and domain exceptions |
| `docs/` | Architecture, feature map, code map, and documentation coverage reports |

## 🏗️ Architecture

NuciDAL supplies generic repository contracts, an in-memory implementation, JSON, XML, and CSV file-backed implementations, and dependency injection registrations for their string-keyed variants. This document records the boundaries that contributors and consumers must preserve: clone-isolated repository state, lazy file hydration, explicit persistence, service lifetime, serialised format contracts, exception semantics, and dependency direction. It describes the library as implemented at version 4.2.0 and does not propose a target architecture.

See the [Architecture Documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 📚 Documentation

| Resource | Description |
|----------|-------------|
| [Architecture Documentation](./ARCHITECTURE.md) | System context, components, runtime flows, boundaries, dependencies, constraints, and extension points |
| [Feature Map](./docs/feature-map.md) | Mapping of user-facing capabilities to implementation locations |
| [Code Map](./docs/code-map.md) | Navigation index for implementation files |
| [Documentation Coverage](./docs/documentation-coverage.md) | XML documentation coverage report |

## 🧾 Data Formats and Protocols

| Format or Protocol | Specification | Purpose |
|--------------------|---------------|---------|
| JSON | RFC 8259 | Entity serialisation for `JsonRepository<T>` |
| XML | W3C XML 1.0 | Entity serialisation for `XmlRepository<T>` |
| CSV | RFC 4180 | Entity serialisation for `CsvRepository<T>` |

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucidal/issues) for problems or suggestions

See the [Contributing Guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucidal/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 🔒 Security

NuciDAL follows coordinated disclosure for security vulnerabilities. Supported versions receive security maintenance via NuGet.org and GitHub Releases.

For information on reporting security vulnerabilities, see the [Security Policy](./SECURITY.md).

## 📄 License

This project is being distributed under the `GNU General Public License v3 or later`.
See [LICENSE](./LICENSE) for additional information.