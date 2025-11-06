
# Brupper .NET Packages Constitution

## Core Principles

### I. Code Quality First (NON-NEGOTIABLE)
Every package MUST maintain enterprise-grade code quality standards. All code MUST pass static analysis with zero warnings, follow consistent coding conventions across all packages, include comprehensive XML documentation for public APIs, and implement proper error handling with meaningful messages. Code analyzers (Roslynator, SonarAnalyzer) are mandatory and MUST NOT be suppressed without documented justification.

**Rationale**: As a developer-focused package collection, poor code quality directly undermines our credibility and adoption.

### II. Test Coverage Standards
All packages MUST maintain minimum 80% code coverage with meaningful tests, not just coverage metrics. Unit tests MUST cover edge cases and error scenarios. Integration tests MUST verify cross-package compatibility and real-world usage patterns. Performance tests MUST validate that utilities don't introduce unexpected overhead in developer applications.

**Rationale**: Developer tools that break or perform poorly damage the trust and productivity of our users.

### III. Developer Experience Consistency
All packages MUST provide consistent APIs following .NET conventions and identical configuration patterns. NuGet packages MUST include IntelliSense-friendly XML documentation, clear quickstart examples, and consistent extension method naming. Breaking changes MUST follow semantic versioning strictly, with migration guides for MAJOR version updates.

**Rationale**: Inconsistent developer experience across our package suite creates friction and confusion for adopting developers.

### IV. Performance Accountability
Performance regressions are considered critical bugs. All packages MUST include benchmark tests for performance-critical paths. Async operations MUST be properly implemented with ConfigureAwait(false) where appropriate. Memory allocations MUST be minimized in hot paths. Performance impact MUST be documented for any utility that adds runtime overhead.

**Rationale**: Developer tools that slow down applications hurt productivity and adoption in performance-sensitive scenarios.

## Developer Experience Requirements

**Discoverability**: Each package MUST have clear purpose documentation explaining when to use it vs alternatives. Package descriptions MUST be concise but complete. Examples MUST demonstrate the most common use cases within 5 lines of code.

**Reliability**: Breaking changes require MAJOR version bumps with clear migration paths. Deprecated features MUST be marked with [Obsolete] attributes and provide alternative recommendations. Dependencies MUST be kept minimal and well-justified.

**Integration**: Cross-package compatibility MUST be maintained. Common scenarios (e.g., ASP.NET Core + EF + Azure) MUST be tested together. Configuration patterns MUST be consistent across packages to reduce learning curve.

## Quality Gates

**Pre-commit**: All commits MUST pass local build, static analysis, and unit tests. Code formatting MUST be consistent via EditorConfig. No TODO comments in main branch without associated issues.

**Pull Request**: All PRs MUST include tests for new functionality, updated documentation for API changes, performance impact assessment for core utilities, and reviewer verification of developer experience quality.

**Release**: All releases MUST pass full test suite including integration tests, performance regression testing, package compatibility verification, and documentation completeness review.

## Governance

This constitution supersedes all other development practices. All pull requests and code reviews MUST verify compliance with these principles. Technical debt that violates these principles MUST be tracked and prioritized for resolution. Exceptions require explicit approval with documented justification and remediation plan.

**Amendment Process**: Constitutional changes require majority team agreement, impact assessment on existing packages, migration plan for affected code, and updated tooling/templates to support new requirements.

