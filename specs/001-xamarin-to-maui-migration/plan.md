# Implementation Plan: Xamarin.Forms to .NET MAUI Migration

**Branch**: `001-xamarin-to-maui-migration` | **Date**: 2025-11-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-xamarin-to-maui-migration/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Migrate existing Xamarin.Forms mobile application to .NET MAUI framework while preserving all functionality, user flows, and UI layouts. Technical approach focuses on framework migration using MAUI single-project structure, Shell Navigation with MVVM Community Toolkit replacing MvvmCross, MAUI Community Toolkit Popup for modals, and Microsoft.Extensions.DependencyInjection for dependency injection. Migration maintains existing namespaces and class names where possible while avoiding deprecated APIs.

## Technical Context

**Language/Version**: C# 12, .NET 9.0  
**Primary Dependencies**: Microsoft.Maui.Controls, Microsoft.Maui.Essentials, CommunityToolkit.Maui, Microsoft.Extensions.DependencyInjection  
**Storage**: Existing data persistence mechanisms (SQLite, preferences, file storage) - framework-agnostic, no migration required  
**Testing**: xUnit with MAUI Test Framework for unit and integration testing, MSTest for platform-specific validation  
**Target Platform**: Multi-platform NuGet package targeting Android, iOS, Windows  
**Project Type**: Mobile library package - single MAUI project structure  
**Performance Goals**: Maintain or improve upon current baseline (startup time, memory usage, UI responsiveness)  
**Constraints**: <10% performance degradation, maintain 100% feature parity, avoid deprecated APIs  
**Scale/Scope**: Single NuGet package with MvvmCross legacy components in _skip folder requiring migration to MAUI native patterns

## Constitution Check

*GATE: Must pass before Phase 0 research. ✅ Re-check after Phase 1 design.*

**Code Quality**: ✅ Static analysis configured via solution-level analyzers, XML documentation required for all public APIs, comprehensive error handling designed for migration scenarios including validation and rollback  
**Test Coverage**: ✅ 80% coverage target set with xUnit + MAUI Test Framework, unit tests for component migration, integration tests for cross-platform functionality, performance tests for baseline comparison  
**Developer Experience**: ✅ Consistent API design maintained from Xamarin.Forms patterns, semantic versioning planned for breaking changes, comprehensive migration guide and quickstart documentation provided  
**Performance**: ✅ Benchmarks planned for startup time and memory usage, async patterns verified in MAUI migration, memory impact assessed for all platform handlers and new dependencies

**Phase 1 Validation**: All constitution requirements satisfied. Research phase resolved all technical unknowns. Data model captures complete migration scope. API contracts define clear interfaces. Quickstart guide enables immediate development team onboarding.

## Project Structure

### Documentation (this feature)

```text
specs/001-xamarin-to-maui-migration/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
# Current MAUI Project Structure
Maui/
├── Brupper.Maui.csproj   # Already MAUI project file
├── ServiceRegister.cs    # DI registration
├── globalusings.cs      # Global using statements
├── Converters/          # Value converters
├── Effects/             # MAUI effects
├── ExtensionMethods/    # Extension methods
├── Fonts/               # Font resources
├── MarkupExtensions/    # XAML markup extensions
├── Models/              # Data models
├── Platforms/           # Platform-specific code
├── Services/            # Application services
├── UiModels/           # UI-specific models
├── ViewModels/         # MAUI ViewModels
├── Views/              # MAUI Views/Pages
└── _skip/              # MvvmCross legacy code requiring migration
    ├── Attributes/     # MvvmCross presentation attributes
    ├── Pages/          # MvxBasePage, MvxPopupPage
    ├── Platforms/      # MvvmCross platform presenters
    ├── Presenters/     # Navigation presenters
    ├── ViewModels/     # MvvmCross ViewModels
    └── Views/          # MvvmCross Views

tests/
├── contract/           # To be created for migration validation
├── integration/        # To be created for cross-platform tests
└── unit/              # To be created for component tests
```

**Structure Decision**: This is a library package migration where the main MAUI project already exists, but contains legacy MvvmCross code in the `_skip` folder that needs to be migrated to MAUI's native patterns. The migration involves integrating the MvvmCross navigation, popup, and MVVM patterns into the existing MAUI structure using Shell Navigation, MAUI Community Toolkit, and Microsoft.Extensions.DependencyInjection.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
