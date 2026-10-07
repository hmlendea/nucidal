# Security Policy

This document describes the security policy for NuciDAL, including supported versions, vulnerability reporting procedures, and disclosure expectations.

## 📑 Table of Contents

- Supported Versions
- Reporting a Vulnerability
- Scope
- Disclosure Policy
- Safe Harbour
- Recognition

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet.org | ✅ |
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nucidal/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Vulnerabilities in the NuciDAL library code (NuciDAL project)
- Vulnerabilities in the NuciDAL unit test code (NuciDAL.UnitTests project)
- Supply chain vulnerabilities in declared NuGet dependencies

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in applications that consume NuciDAL
- Vulnerabilities in the .NET runtime or SDK
- Vulnerabilities in third-party tools or IDEs used for development
- Social engineering or physical attacks against maintainers

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.