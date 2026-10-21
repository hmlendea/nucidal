# Privacy and Personal Data

NuciDAL is a .NET library providing generic repository abstractions and file-backed persistence (JSON, XML, CSV). The library itself does not collect, process, transmit, or store personal data. All data handling occurs in the consuming application, which is outside the scope of this document.

**Information reviewed:** 2026-10-07

## 📑 Table of Contents

- What This Document Covers
- Self-Hosted Deployments
- Data We Handle
- Processing and Use
- Storage, Retention, and Deletion
- External Processing and Integrations
- Document Changes
- Contact

## 🔎 What This Document Covers

This document describes how NuciDAL at https://github.com/hmlendea/nucidal handles personal data. NuciDAL is a library with no runtime service, telemetry, user accounts, or external integrations. It does not process personal data. Consuming applications that use NuciDAL to persist entities are responsible for their own data handling.

## 🏠 Self-Hosted Deployments

NuciDAL is a library distributed via NuGet and GitHub Releases. It is not a deployable service. Applications that reference NuciDAL run in the operator's environment. The library does not send data to project maintainers or external services. No telemetry, update checks, crash reports, or network calls are performed by the library.

## 📥 Data We Handle

### Data Provided to the Application

NuciDAL does not request or receive personal data. The library provides repository interfaces (`IRepository<T>`, `IFileRepository<T>`) that consuming applications use to store their own entity types. Any personal data in those entities is provided and controlled by the consuming application.

### Data Generated or Collected by the Application

NuciDAL does not generate or collect personal data, telemetry, logs, audit trails, crash reports, or update-check data.

### Data Received from Integrations

NuciDAL has no built-in integrations that receive personal data. It depends on `Microsoft.Extensions.DependencyInjection.Abstractions` and `NuciExtensions` for DI contracts and entity cloning utilities; neither dependency transmits data.

## 🧭 Processing and Use

NuciDAL performs no processing of personal data. The library provides in-memory and file-backed repository implementations that serialize and deserialize entities to local files (JSON, XML, CSV) or keep them in memory. All processing is local to the consuming application's process.

## 🗄️ Storage, Retention, and Deletion

NuciDAL does not store data independently. File-backed repositories (`JsonRepository<T>`, `XmlRepository<T>`, `CsvRepository<T>`) write to files at paths supplied by the consuming application. The consuming application controls the file location, retention, backup, and deletion. In-memory repositories (`Repository<T>`) hold data only for the process lifetime. The library has no databases, caches, or background persistence.

## 🔗 External Processing and Integrations

NuciDAL has no built-in external data transfers. The NuGet package has no runtime dependencies that process data.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | N/A | N/A | N/A |

## 🛡️ Data Protection and Security

NuciDAL does not handle personal data, so no data-specific safeguards apply. The library is distributed as a NuGet package and GitHub Release artifacts. Consuming applications are responsible for securing their own data, file permissions, and runtime environment.

## 🔄 Document Changes

Update this document if NuciDAL adds telemetry, network calls, external integrations, or any data-handling behaviour. The current version is published at https://github.com/hmlendea/nucidal/blob/master/PRIVACY.md.

## 📬 Contact

For questions about NuciDAL's data handling (or lack thereof), open an issue at https://github.com/hmlendea/nucidal/issues. Do not send passwords, access tokens, or other secrets.