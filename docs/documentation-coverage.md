# Documentation Coverage

This audit records the current documentation and test traceability state. A documented gap is intentional evidence, not a claim that the behaviour is absent from the product.

## Coverage Model

The repository has one packable library project and one NUnit test project. There is no executable host, web endpoint, database, scheduler, worker, container, or deployment stack owned by this repository.

| Area | Documentation | Implementation | Tests | Status |
| --- | --- | --- | --- | --- |
| Entity identity and equality | [Feature map](feature-map.md#entity-identity-and-equality), [Architecture](../ARCHITECTURE.md#data-objects) | [`EntityBase.cs`](../NuciDAL/DataObjects/EntityBase.cs), `EntityBase<TKey>`, `EntityBase` | [`EntityBaseTests.cs`](../NuciDAL.UnitTests/DataObjects/EntityBaseTests.cs) | Direct unit coverage |
| Repository contract and in-memory operations | [Feature map](feature-map.md#repository-contracts-and-operations), [Architecture](../ARCHITECTURE.md#repository-contracts-and-implementations) | [`IRepository.cs`](../NuciDAL/Repositories/IRepository.cs), [`Repository.cs`](../NuciDAL/Repositories/Repository.cs) | Repository add, contains, count, find, get, remove, and update test classes | Broad unit coverage |
| Clone isolation | [Feature map](feature-map.md#clone-isolation), [Architecture data architecture](../ARCHITECTURE.md#data-architecture) | [`Repository.cs`](../NuciDAL/Repositories/Repository.cs), `CloneEntity` | Add, get, and update clone tests | Direct in-memory coverage; file lifecycle gap remains |
| File repository lifecycle | [Feature map](feature-map.md#file-persistence), [Architecture lifecycle](../ARCHITECTURE.md#file-repository-lifecycle) | [`FileRepository.cs`](../NuciDAL/Repositories/FileRepository.cs), `JsonRepository`, `XmlRepository`, `CsvRepository` | No direct hydration or save tests | Documented implementation, missing direct tests |
| JSON, XML, and CSV collection I/O | [Feature map](feature-map.md#serialisation-and-file-io), [Architecture format semantics](../ARCHITECTURE.md#file-format-semantics) | [`JsonFileCollection.cs`](../NuciDAL/IO/JsonFileCollection.cs), [`XmlFileCollection.cs`](../NuciDAL/IO/XmlFileCollection.cs), [`CsvFile.cs`](../NuciDAL/IO/CsvFile.cs) | None | Documented implementation, missing direct tests |
| Standalone object I/O and Windows-1252 output | [Feature map](feature-map.md#non-repository-utilities) | [`JsonFileObject.cs`](../NuciDAL/IO/JsonFileObject.cs), [`XmlFileObject.cs`](../NuciDAL/IO/XmlFileObject.cs), [`Windows1252File.cs`](../NuciDAL/IO/Windows1252File.cs) | None | Documented implementation, missing direct tests |
| Dependency injection composition | [Feature map](feature-map.md#dependency-injection-composition), [Architecture DI](../ARCHITECTURE.md#dependency-injection) | [`RepositoryServiceCollectionExtensions.cs`](../NuciDAL/DependencyInjection/RepositoryServiceCollectionExtensions.cs), `AddRepository`, `AddJsonRepository`, `AddXmlRepository`, `AddCsvRepository` | [`RepositoryServiceCollectionExtensionsTests.cs`](../NuciDAL.UnitTests/DependencyInjection/RepositoryServiceCollectionExtensionsTests.cs) | Direct unit coverage |
| Exception model | [Feature map](feature-map.md#exception-model), [Architecture error handling](../ARCHITECTURE.md#error-handling) | [`EntityException.cs`](../NuciDAL/Repositories/EntityException.cs), typed exception files | Three typed exception test classes plus repository failure tests | Direct unit coverage |
| Packaging and CI | [Code map](code-map.md#configuration-packaging-and-operations), [README development](../README.md#development) | [`NuciDAL.csproj`](../NuciDAL/NuciDAL.csproj), [`NuciDAL.UnitTests.csproj`](../NuciDAL.UnitTests/NuciDAL.UnitTests.csproj), [`dotnet.yml`](../.github/workflows/dotnet.yml) | CI executes restore, build, and test commands | Configuration and operational references present |

## Feature Traceability Audit

### Repository Operations

- **Entry:** public `IRepository<TKey, TDataObject>` calls from a consuming process.
- **Principal implementation:** [`Repository.cs`](../NuciDAL/Repositories/Repository.cs), `Repository<TKey, TDataObject>`.
- **Supporting implementation:** [`EntityBase.cs`](../NuciDAL/DataObjects/EntityBase.cs), `NuciExtensions` cloning extensions, `ConcurrentDictionary`.
- **Data:** `EntityBase<TKey>.Id` and derived entity properties.
- **Configuration and registration:** direct construction or `AddRepository` in [`RepositoryServiceCollectionExtensions.cs`](../NuciDAL/DependencyInjection/RepositoryServiceCollectionExtensions.cs).
- **Persistence:** memory only unless a `FileRepository` is selected.
- **Tests:** operation-specific repository test files listed in [Code Map](code-map.md#test-project).
- **Documentation:** [Feature Map repository operations](feature-map.md#repository-contracts-and-operations), [Architecture](../ARCHITECTURE.md#data-architecture).

### File Persistence And Serialisation

- **Entry:** concrete file repository construction, first public repository operation, or `IFileRepository.SaveChanges()`.
- **Principal implementation:** [`FileRepository.cs`](../NuciDAL/Repositories/FileRepository.cs).
- **Supporting implementation:** format repositories and collection helpers under [`NuciDAL/Repositories`](../NuciDAL/Repositories) and [`NuciDAL/IO`](../NuciDAL/IO).
- **Data:** JSON, XML, or CSV file representation of the consumer entity.
- **Configuration:** constructor file path; CSV separator; JSON and XML serializer settings in their helper classes.
- **Registration:** `AddJsonRepository`, `AddXmlRepository`, and `AddCsvRepository`.
- **Tests:** only DI construction and deferred-path tests currently exist.
- **Gaps:** hydration, persistence, malformed input, file errors, external modifications, duplicate file identifiers, and concurrency lack direct tests.
- **Documentation:** [Feature Map file persistence](feature-map.md#file-persistence), [serialisation](feature-map.md#serialisation-and-file-io), and [Architecture](../ARCHITECTURE.md#file-format-semantics).

### Dependency Injection

- **Entry:** consumer invokes one of the four extension methods.
- **Principal implementation:** [`RepositoryServiceCollectionExtensions.cs`](../NuciDAL/DependencyInjection/RepositoryServiceCollectionExtensions.cs).
- **Registration:** singleton service descriptors for repository interfaces.
- **Configuration:** consumer `Func<string>` path provider; CSV generic constructor constraint.
- **Tests:** [`RepositoryServiceCollectionExtensionsTests.cs`](../NuciDAL.UnitTests/DependencyInjection/RepositoryServiceCollectionExtensionsTests.cs).
- **Documentation:** [Feature Map DI](feature-map.md#dependency-injection-composition), [Architecture DI](../ARCHITECTURE.md#dependency-injection).

## Algorithm Traceability Audit

| Algorithm or procedure | Inputs and outputs | Branches and invariants | Implementation | Tests |
| --- | --- | --- | --- | --- |
| Clone isolation | Entity -> equivalent independent entity | Serialised values remain equivalent; object references are not shared | [`Repository.cs`](../NuciDAL/Repositories/Repository.cs), `CloneEntity` | Add, get, and update clone tests |
| Lazy hydration | File path -> populated dictionary | Double-check lock; one successful load; duplicate identifier rejects | [`FileRepository.cs`](../NuciDAL/Repositories/FileRepository.cs), `LoadEntitiesIfNeeded` | No direct algorithm test |
| Duplicate detection | Loaded entity sequence -> dictionary or `DuplicateEntityException` | `TryAdd` failure identifies repeated key | [`FileRepository.cs`](../NuciDAL/Repositories/FileRepository.cs) | Exception construction tests; no file-load integration test |
| Predicate snapshot | Repository state and predicate -> enumerable result | Snapshot is materialised before later repository mutations | [`Repository.cs`](../NuciDAL/Repositories/Repository.cs), `Find` | [`RepositoryFindTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryFindTests.cs) |
| CSV property reordering | Reflected properties -> reordered array | Final reflected property is moved to the first position; implementation carries a known hack | [`CsvFile.cs`](../NuciDAL/IO/CsvFile.cs), `GetReorderedProperties` | None |

## Test And Operational Coverage

### Directly Tested

- Entity equality, symmetry, null handling, type distinction, and hash consistency.
- In-memory add, update, remove, try variants, count, contains, lookup, random selection, first selection, and predicate composition.
- Clone-on-add, clone-on-get, clone-on-update, and mutation isolation for in-memory state.
- Typed exception metadata, message content, constructor overloads, and inner exceptions.
- DI null guards, returned service collections, resolved implementation types, singleton identity, and deferred path provider timing.

### Missing Direct Tests

- JSON, XML, and CSV file loading and saving through both helpers and repositories.
- `IFileRepository.SaveChanges()` observable file replacement.
- Duplicate identifiers encountered during actual file hydration.
- Malformed JSON/XML/CSV, CSV comments, empty files, field-count errors, null values, conversion errors, missing paths, permissions, and encoding failures.
- Concurrent repository operations, `SyncRoot`, and the double-check locking procedure.
- Generic non-string key repository operations despite the `IntKeyEntityDataObject` stub.
- Standalone JSON/XML object helpers and Windows-1252 synchronous/asynchronous output.
- Failure while invoking a deferred DI store-path provider.

### Operational Boundaries

- CI is implemented only by [`dotnet.yml`](../.github/workflows/dotnet.yml), which restores, builds, and tests the solution.
- Packaging is configured in [`NuciDAL.csproj`](../NuciDAL/NuciDAL.csproj); no release workflow, deployment definition, container, service definition, or runtime host is present.
- Local filesystem access is the only persistence integration; the consumer owns path permissions, confidentiality, backup, and multi-process coordination.

## Inverse Audit

| Implementation area | Documented purpose | Related tests |
| --- | --- | --- |
| `NuciDAL/DataObjects/*` | Entity identity and equality | `EntityBaseTests` |
| `NuciDAL/Repositories/I*` | Public repository contracts | Operation and DI tests exercise contracts |
| `NuciDAL/Repositories/Repository.cs` | In-memory repository semantics and cloning | Repository operation tests |
| `NuciDAL/Repositories/FileRepository.cs` and concrete file repositories | Lazy file lifecycle and format adapters | No direct lifecycle tests |
| `NuciDAL/Repositories/*Exception.cs` | Typed repository failures | Exception tests and repository failure cases |
| `NuciDAL/DependencyInjection/*` | Service registration and composition | DI registration tests |
| `NuciDAL/IO/*` | Serialisation, parsing, file access, and encoding | No direct I/O tests |
| `NuciDAL.UnitTests/*` | Behavioural verification | Mapped by [Code Map](code-map.md#test-project) |
| `.github/workflows/dotnet.yml` | CI verification | Workflow itself; no workflow-specific test |

The inverse audit identifies no production source directory without documentation. It identifies the file I/O and concurrency areas as the principal implementation units without direct behavioural tests.
