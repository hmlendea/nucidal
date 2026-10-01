# Feature Map

This map provides the concept-to-code route for NuciDAL. Links point to repository files; symbols identify the relevant implementation within each file.

## Repository Contracts And Operations

| Field | Reference |
| --- | --- |
| Capability | Generic keyed repository API: add, update, remove, lookup, count, random selection, and predicate queries. |
| Purpose | Provide one storage-independent API for entity access. |
| Principal documentation | [README capabilities and usage](../README.md), [architecture repository contracts](../ARCHITECTURE.md) |
| Entry point | Consumer calls `IRepository<TKey, TDataObject>` methods. |
| Principal implementation | [`NuciDAL/Repositories/Repository.cs`](../NuciDAL/Repositories/Repository.cs): `Repository<TKey, TDataObject>`; `Add`, `TryAdd`, `ContainsId`, `Get`, `GetRandom`, `GetFirst`, `TryGet`, `TryGetFirst`, `GetAll`, `Find`, `Update`, `TryUpdate`, `Remove`, `TryRemove`, `EntitiesCount`. |
| Supporting implementation | [`NuciDAL/Repositories/IRepository.cs`](../NuciDAL/Repositories/IRepository.cs): `IRepository<TKey, TDataObject>`. [`NuciDAL/DataObjects/EntityBase.cs`](../NuciDAL/DataObjects/EntityBase.cs): entity constraint and identifier. |
| Domain logic | `Repository<TKey, TDataObject>.ExecuteWrite`, `ExecuteReadOperation`, `CloneEntity`, and keyed `ConcurrentDictionary` state. |
| Data models | `EntityBase<TKey>` and the consumer's derived `TDataObject`. |
| Persistence | In-memory only in `Repository<TKey, TDataObject>`; file persistence is supplied by [File Persistence](#file-persistence). |
| Validation and errors | `EntityAlreadyExistsException` for duplicate add; `EntityNotFoundException` for missing get, update, or remove. |
| Tests | [`RepositoryAddTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryAddTests.cs), [`RepositoryContainsTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryContainsTests.cs), [`RepositoryCountTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryCountTests.cs), [`RepositoryFindTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryFindTests.cs), [`RepositoryGetTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryGetTests.cs), [`RepositoryRemoveTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryRemoveTests.cs), [`RepositoryUpdateTests.cs`](../NuciDAL.UnitTests/Repositories/RepositoryUpdateTests.cs). |
| Operational implementation | [`NuciDAL/NuciDAL.csproj`](../NuciDAL/NuciDAL.csproj) packages the library; [`dotnet.yml`](../.github/workflows/dotnet.yml) restores, builds, and tests it. |

## Entity Identity And Equality

| Field | Reference |
| --- | --- |
| Capability | Generic entity identity, reflection-based equality, hashing, and JSON string representation. |
| Purpose | Supply the base contract required by repositories and stable value comparisons. |
| Principal implementation | [`NuciDAL/DataObjects/EntityBase.cs`](../NuciDAL/DataObjects/EntityBase.cs): `EntityBase<TKey>`, `EntityBase`, `Equals`, `GetHashCode`, and `ToString`. |
| Supporting implementation | `NuciExtensions` JSON extensions used by `ToString` and repository cloning. |
| Entry point and composition | Consumer derives a data object from `EntityBase<TKey>` or the string-keyed `EntityBase`; no application startup registration is required. |
| Data models | `EntityBase<TKey>.Id`; consumer-defined public properties. |
| Tests | [`EntityBaseTests.cs`](../NuciDAL.UnitTests/DataObjects/EntityBaseTests.cs): `EntityBaseTests`, including null, reference, type, property, symmetry, and hash cases. |
| Coverage gap | `IntKeyEntityDataObject` exists in [`IntKeyEntityDataObject.cs`](../NuciDAL.UnitTests/Stubs/IntKeyEntityDataObject.cs), but generic-key repository behaviour is not directly exercised. |

## Clone Isolation

| Field | Reference |
| --- | --- |
| Capability | Clone entities at repository ingress and egress using a JSON round trip. |
| Purpose | Prevent callers from mutating repository state through retained object references. |
| Principal implementation | [`NuciDAL/Repositories/Repository.cs`](../NuciDAL/Repositories/Repository.cs): `CloneEntity` and calls from add, update, get, collection, and remove operations. |
| Supporting implementation | `NuciExtensions` `ToJson()` and `FromJson<TDataObject>()`. |
| Entry point | `IRepository` add, update, get, find, and remove operations. |
| Tests | `RepositoryAddTests.GivenEntity_WhenAdded_ThenStoredEntityIsAClone`; `RepositoryGetTests.GivenEntity_WhenGetIsCalled_ThenReturnsCloneNotOriginalReference`; `RepositoryGetTests.GivenReturnedCloneIsModified_WhenGetIsCalled_ThenStoredEntityRemainsUnchanged`; `RepositoryUpdateTests.GivenEntity_WhenUpdateIsCalled_ThenStoresCloneNotReference`. |
| Coverage gap | File-repository hydration and save clone boundaries are not directly tested. |

## File Persistence

| Field | Reference |
| --- | --- |
| Capability | Lazy loading from a complete file and explicit complete-file persistence. |
| Purpose | Add durable JSON, XML, or CSV storage while retaining repository semantics. |
| Entry point | Consumer constructs `JsonRepository<TDataObject>`, `XmlRepository<TDataObject>`, or `CsvRepository<TDataObject>` and calls repository methods or `IFileRepository.SaveChanges()`. |
| Principal implementation | [`NuciDAL/Repositories/FileRepository.cs`](../NuciDAL/Repositories/FileRepository.cs): `FileRepository<TKey, TDataObject>`, `LoadEntitiesIfNeeded`, `FetchEntitiesFromFile`, `PerformFileSave`, and `SaveChanges`. |
| Supporting implementation | [`JsonRepository.cs`](../NuciDAL/Repositories/JsonRepository.cs): `JsonRepository`; [`XmlRepository.cs`](../NuciDAL/Repositories/XmlRepository.cs): `XmlRepository`; [`CsvRepository.cs`](../NuciDAL/Repositories/CsvRepository.cs): `CsvRepository`. |
| Domain logic | `FileRepository` uses `loadedEntities`, `SyncRoot`, double-checked locking, and duplicate-key insertion. |
| Data and serialisation | [`IFileRepository.cs`](../NuciDAL/Repositories/IFileRepository.cs): `SaveChanges`; collection helpers in [`NuciDAL/IO`](../NuciDAL/IO). |
| Persistence | Format-specific `FetchEntitiesFromFile` and `PerformFileSave` implementations write to the configured local path. |
| Validation and errors | `DuplicateEntityException` on duplicate identifiers during hydration; underlying parsing and filesystem exceptions remain format-specific. |
| Tests | DI construction tests cover type and lifetime only in [`RepositoryServiceCollectionExtensionsTests.cs`](../NuciDAL.UnitTests/DependencyInjection/RepositoryServiceCollectionExtensionsTests.cs). |
| Coverage gap | No direct tests for hydration, `SaveChanges`, malformed data, missing files, permissions, external file changes, or concurrent loading. |

## Serialisation And File I/O

| Field | Reference |
| --- | --- |
| Capability | JSON, XML, CSV collection/object serialisation and Windows-1252 text output. |
| Purpose | Convert CLR entities or objects to local file representations. |
| Principal implementation | [`JsonFileCollection.cs`](../NuciDAL/IO/JsonFileCollection.cs): `JsonFileCollection<T>.LoadEntities` and `SaveEntities`; [`XmlFileCollection.cs`](../NuciDAL/IO/XmlFileCollection.cs): `XmlFileCollection<T>.LoadEntities` and `SaveEntities`; [`CsvFile.cs`](../NuciDAL/IO/CsvFile.cs): `CsvFile<TDataObject>.LoadEntities` and `SaveEntities`. |
| Supporting implementation | [`JsonFileObject.cs`](../NuciDAL/IO/JsonFileObject.cs): `JsonFileObject<T>.Read` and `Write`; [`XmlFileObject.cs`](../NuciDAL/IO/XmlFileObject.cs): `XmlFileObject<T>.Read` and `Write`; [`Windows1252File.cs`](../NuciDAL/IO/Windows1252File.cs): `WriteAllText` and `WriteAllTextAsync`. |
| Configuration | File path is constructor input; CSV field separator is constructor input with comma default; JSON uses camelCase and indented output; XML uses `XmlSerializer`; CSV treats `#` lines as comments. |
| Algorithm | CSV reflection property ordering is implemented by `CsvFile<TDataObject>.GetReorderedProperties`; parsing reports line context through `SerializationException`. |
| Tests | No direct tests for these helpers. Repository and DI tests only cover the surrounding API. |
| Coverage gap | Empty files, comments, field-count mismatch, null values, conversion errors, malformed JSON/XML, encoding, and file I/O failures lack direct tests. |

## Dependency Injection Composition

| Field | Reference |
| --- | --- |
| Capability | Register string-keyed in-memory and file-backed repositories as singleton services. |
| Purpose | Compose repositories through `IServiceCollection` without exposing concrete construction to consumers. |
| Entry point | [`RepositoryServiceCollectionExtensions.cs`](../NuciDAL/DependencyInjection/RepositoryServiceCollectionExtensions.cs): `AddRepository`, `AddJsonRepository`, `AddXmlRepository`, `AddCsvRepository`. |
| Principal implementation | `RepositoryServiceCollectionExtensions.AddFileRepository`, which validates inputs and creates the singleton from a deferred path provider. |
| Registration | `IRepository<TDataObject>` -> `Repository<TDataObject>`; `IFileRepository<TDataObject>` -> selected format repository; all singleton lifetime. |
| Configuration | Consumer-supplied `Func<string> storePathProvider`; CSV additionally requires `new()` on the data object. |
| Tests | [`RepositoryServiceCollectionExtensionsTests.cs`](../NuciDAL.UnitTests/DependencyInjection/RepositoryServiceCollectionExtensionsTests.cs): `RepositoryServiceCollectionExtensionsTests`, including collection return, null guards, type resolution, singleton identity, and deferred path invocation. |
| Operational implementation | [`NuciDAL/NuciDAL.csproj`](../NuciDAL/NuciDAL.csproj) references `Microsoft.Extensions.DependencyInjection.Abstractions`; test project references the concrete DI package. |

## Exception Model

| Field | Reference |
| --- | --- |
| Capability | Typed exceptions carrying entity identifier and type context. |
| Principal implementation | [`EntityException.cs`](../NuciDAL/Repositories/EntityException.cs): `EntityException`; [`EntityAlreadyExistsException.cs`](../NuciDAL/Repositories/EntityAlreadyExistsException.cs): `EntityAlreadyExistsException`; [`EntityNotFoundException.cs`](../NuciDAL/Repositories/EntityNotFoundException.cs): `EntityNotFoundException`; [`DuplicateEntityException.cs`](../NuciDAL/Repositories/DuplicateEntityException.cs): `DuplicateEntityException`. |
| Callers | `Repository` raises existing/not-found exceptions; `FileRepository` raises duplicate exceptions during load. |
| Tests | [`EntityAlreadyExistsExceptionTests.cs`](../NuciDAL.UnitTests/Repositories/EntityAlreadyExistsExceptionTests.cs), [`EntityNotFoundExceptionTests.cs`](../NuciDAL.UnitTests/Repositories/EntityNotFoundExceptionTests.cs), and [`DuplicateEntityExceptionTests.cs`](../NuciDAL.UnitTests/Repositories/DuplicateEntityExceptionTests.cs). |

## Non-Repository Utilities

| Capability | Implementation | Tests and gaps |
| --- | --- | --- |
| Standalone JSON object I/O | [`JsonFileObject.cs`](../NuciDAL/IO/JsonFileObject.cs), `JsonFileObject<T>` | No direct tests. |
| Standalone XML object I/O | [`XmlFileObject.cs`](../NuciDAL/IO/XmlFileObject.cs), `XmlFileObject<T>` | No direct tests. |
| Windows-1252 output | [`Windows1252File.cs`](../NuciDAL/IO/Windows1252File.cs), `WriteAllText`, `WriteAllTextAsync` | No direct tests. |

## Algorithm Map

### Clone Isolation

- **Inputs:** consumer entity or repository entity.
- **Output:** deserialised `TDataObject` instance with equivalent serialised values.
- **Procedure:** `Repository.CloneEntity` calls `ToJson`, then `FromJson<TDataObject>`.
- **Invariant:** repository state does not share the cloned entity reference with the caller.
- **Callers:** repository add, update, get, find, random, and remove paths.
- **Tests:** clone tests listed in [Clone Isolation](#clone-isolation).
- **Complexity:** proportional to the serialised entity graph size; exact cost is delegated to `NuciExtensions` and `System.Text.Json`.

### Lazy File Hydration

- **Inputs:** configured file path and repository instance.
- **Output:** populated in-memory dictionary, or the original read/parse exception.
- **Precondition:** first file-backed repository operation.
- **Procedure:** `LoadEntitiesIfNeeded` checks `loadedEntities`, locks `SyncRoot`, checks again, fetches entities, inserts each by identifier, and marks the instance loaded.
- **Important branches:** already loaded; duplicate identifier; read or parse failure; empty source.
- **Invariant:** successful loading occurs once per repository instance.
- **Callers and consumers:** file repository public operations and `SaveChanges`; concrete format repositories provide hooks.
- **Tests:** no direct algorithm tests; DI tests only verify construction and deferred path invocation.

### CSV Property Ordering

- **Inputs:** reflected public entity properties.
- **Output:** reordered property array used for CSV header/value processing.
- **Implementation:** `CsvFile<TDataObject>.GetReorderedProperties` in [`CsvFile.cs`](../NuciDAL/IO/CsvFile.cs).
- **Assumption:** the final reflected property is moved to the first position; the implementation contains a known `VERY HACKY` note.
- **Tests:** none; this is a documented test gap.
