# Data Model: Xamarin.Forms to .NET MAUI Migration

**Feature**: Xamarin.Forms to .NET MAUI Migration  
**Created**: 2025-11-06  
**Status**: Phase 1 Design

## Core Entities

### Mobile Application Package
**Purpose**: The Brupper.Maui NuGet package providing MAUI utilities and components  
**Key Attributes**:
- Package identifier and version
- Supported target frameworks (net9.0-android, net9.0-ios, net9.0-windows)
- Dependencies on core MAUI packages
- Platform-specific implementations
- Public API surface

**Validation Rules**:
- Must target supported MAUI frameworks only
- Package version must follow semantic versioning
- All public APIs must have XML documentation
- Platform-specific code must be conditionally compiled

**State Transitions**:
- Draft → Testing → Published → Deprecated

### Platform Implementation
**Purpose**: Platform-specific code implementations for Android, iOS, Windows  
**Key Attributes**:
- Target platform identifier
- Platform-specific dependencies
- Handler implementations
- Native API integrations
- Resource files

**Relationships**:
- Belongs to Mobile Application Package
- Implements Common Interfaces
- References Platform Dependencies

**Validation Rules**:
- Must implement all required platform contracts
- Platform code must be isolated in appropriate folders
- No cross-platform dependencies in platform-specific code

### MAUI Handler
**Purpose**: Platform customization components replacing Xamarin.Forms renderers  
**Key Attributes**:
- Handler name and type
- Target control type
- Platform-specific implementation
- Property mappings
- Event mappings

**Relationships**:
- Replaces Legacy Renderer
- Belongs to Platform Implementation
- Implements Platform Interface

**Validation Rules**:
- Must inherit from appropriate MAUI handler base class
- Property mappings must be complete
- Must handle lifecycle events properly

### Legacy Renderer (Migration Entity)
**Purpose**: Xamarin.Forms renderers that need conversion to MAUI handlers  
**Key Attributes**:
- Renderer class name
- Target control type
- Platform-specific implementation
- Custom properties
- Migration status

**Relationships**:
- Located in _skip folder
- To be replaced by MAUI Handler
- May have dependencies on Xamarin.Forms APIs

**State Transitions**:
- Identified → Analyzed → Migrated → Validated → Removed

### Package Dependency
**Purpose**: External NuGet packages required by the Brupper.Maui package  
**Key Attributes**:
- Package name and version
- Framework compatibility
- Migration status (Xamarin.Forms vs MAUI)
- Required vs optional dependency

**Validation Rules**:
- Must be compatible with target MAUI frameworks
- Should use stable versions for production
- Must not reference deprecated Xamarin.Forms packages

**State Transitions**:
- Xamarin.Forms → MAUI Equivalent → Validated → Integrated

### Test Suite
**Purpose**: Automated tests validating MAUI functionality and migration success  
**Key Attributes**:
- Test project structure
- Test categories (unit, integration, platform)
- Coverage metrics
- Platform-specific test configurations

**Relationships**:
- Tests Mobile Application Package
- Validates Platform Implementations
- Verifies Migration Success

**Validation Rules**:
- Must achieve minimum 80% code coverage
- Platform-specific tests for each target platform
- Performance tests to validate no regression

## Data Relationships

```
Mobile Application Package
├── Platform Implementation (Android)
│   ├── MAUI Handler (Multiple)
│   └── Platform Dependencies
├── Platform Implementation (iOS)  
│   ├── MAUI Handler (Multiple)
│   └── Platform Dependencies
├── Platform Implementation (Windows)
│   ├── MAUI Handler (Multiple)
│   └── Platform Dependencies
├── Package Dependencies (Multiple)
├── Legacy Renderers (in _skip folder)
│   └── Migration Tasks
└── Test Suite
    ├── Unit Tests
    ├── Integration Tests
    └── Platform Tests
```

## Migration-Specific Entities

### Migration Task
**Purpose**: Specific work items for converting legacy code to MAUI  
**Key Attributes**:
- Task identifier
- Source component (renderer, page, effect)
- Target implementation approach
- Complexity level
- Dependencies
- Completion status

**State Transitions**:
- Identified → Planned → In Progress → Testing → Complete

### API Change Documentation
**Purpose**: Documentation of breaking changes and migration guidance  
**Key Attributes**:
- Changed API names
- Before/after code examples
- Migration instructions
- Impact assessment
- Consumer guidance

### Platform Compatibility Matrix
**Purpose**: Tracking of feature support across target platforms  
**Key Attributes**:
- Feature name
- Android support level
- iOS support level  
- Windows support level
- Known limitations
- Workarounds

## Key Business Rules

1. **Backward Compatibility**: Package must maintain API compatibility where possible
2. **Platform Parity**: Core features must work consistently across all target platforms
3. **Performance**: Migrated package must not introduce performance regressions
4. **Quality**: All migrated code must meet constitution quality standards
5. **Documentation**: Breaking changes must be clearly documented with migration guidance

## Data Validation Constraints

- All public APIs must have comprehensive XML documentation
- Platform-specific code must include appropriate platform guards
- Test coverage must be maintained or improved during migration
- Package dependencies must be MAUI-compatible and up-to-date
- Legacy code in _skip folder must be either migrated or removed
- Migration tasks must be tracked to completion
- Performance benchmarks must validate no significant regression