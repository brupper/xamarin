---

description: "Task list for Xamarin.Forms to .NET MAUI Migration"
---

# Tasks: Xamarin.Forms to .NET MAUI Migration

**Input**: Design documents from `/specs/001-xamarin-to-maui-migration/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Included as this is a migration project requiring comprehensive validation

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Based on plan.md structure - MAUI single-project package targeting Android, iOS, Windows

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and migration preparation

- [ ] T001 Create test project structure at Brupper.Maui.Tests/
- [ ] T002 Install .NET 9 SDK and MAUI workload dependencies per quickstart.md
- [ ] T003 [P] Configure xUnit testing framework in Brupper.Maui.Tests/Brupper.Maui.Tests.csproj
- [ ] T004 [P] Establish performance baseline measurements for startup time and memory usage
- [ ] T005 [P] Audit and document all legacy code in Maui/_skip/ folder structure
- [ ] T006 [P] Create migration tracking spreadsheet based on data-model.md entities

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core MAUI infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T007 Verify all package references in Maui/Brupper.Maui.csproj are MAUI-compatible
- [ ] T008 Update Microsoft.Extensions.DependencyInjection integration in Maui/ServiceRegister.cs
- [ ] T009 [P] Create base MAUI handler infrastructure in Maui/Platforms/ folders
- [ ] T010 [P] Update global using statements in Maui/globalusings.cs for MAUI APIs
- [ ] T011 [P] Configure static analysis rules for MAUI compliance validation
- [ ] T012 [P] Create migration validation framework in Brupper.Maui.Tests/Infrastructure/

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Core App Functionality Preservation (Priority: P1) 🎯 MVP

**Goal**: Migrate MvvmCross components from _skip folder to MAUI native patterns while preserving all functionality

**Independent Test**: All existing app functionality works identically after migration with MAUI patterns

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T013 [P] [US1] Create MvvmCross component inventory test in Brupper.Maui.Tests/US1/ComponentInventoryTests.cs
- [ ] T014 [P] [US1] Create navigation functionality test in Brupper.Maui.Tests/US1/NavigationPreservationTests.cs
- [ ] T015 [P] [US1] Create popup functionality test in Brupper.Maui.Tests/US1/PopupFunctionalityTests.cs
- [ ] T016 [P] [US1] Create ViewModel binding test in Brupper.Maui.Tests/US1/ViewModelBindingTests.cs

### Implementation for User Story 1

- [ ] T017 [P] [US1] Migrate MvxBasePage to MAUI ContentPage pattern in Maui/Views/BasePage.cs
- [ ] T018 [P] [US1] Migrate MvxPopupPage to MAUI Community Toolkit Popup in Maui/Views/BasePopup.cs
- [ ] T019 [US1] Implement Shell Navigation replacement for MvvmCross navigation in Maui/Services/NavigationService.cs
- [ ] T020 [US1] Convert MvvmCross ViewModels to MVVM Community Toolkit ObservableObject in Maui/ViewModels/
- [ ] T021 [US1] Update presentation attributes to MAUI equivalents in Maui/Attributes/
- [ ] T022 [US1] Migrate MvvmCross lifecycle events to MAUI Page Lifecycle in Maui/Views/BasePage.cs
- [ ] T023 [US1] Update dependency injection from MvvmCross to Microsoft.Extensions.DependencyInjection in Maui/ServiceRegister.cs
- [ ] T024 [US1] Convert BackPressedCommand implementations to MAUI patterns
- [ ] T025 [US1] Remove all Xamarin.Forms references from migrated components
- [ ] T026 [US1] Clean up successfully migrated components from Maui/_skip/ folder

**Checkpoint**: At this point, all core MvvmCross functionality should be replaced with MAUI native patterns

---

## Phase 4: User Story 2 - Multi-Platform Compatibility (Priority: P2)

**Goal**: Ensure migrated MAUI application runs successfully on Android, iOS, and Windows with platform-appropriate behavior

**Independent Test**: Deploy to each platform and verify platform-specific features work correctly

### Tests for User Story 2

- [ ] T027 [P] [US2] Create Android platform compatibility test in Brupper.Maui.Tests/US2/AndroidCompatibilityTests.cs
- [ ] T028 [P] [US2] Create iOS platform compatibility test in Brupper.Maui.Tests/US2/iOSCompatibilityTests.cs
- [ ] T029 [P] [US2] Create Windows platform compatibility test in Brupper.Maui.Tests/US2/WindowsCompatibilityTests.cs
- [ ] T030 [P] [US2] Create cross-platform UI consistency test in Brupper.Maui.Tests/US2/CrossPlatformUITests.cs

### Implementation for User Story 2

- [ ] T031 [P] [US2] Convert platform-specific renderers to MAUI handlers for Android in Maui/Platforms/Android/
- [ ] T032 [P] [US2] Convert platform-specific renderers to MAUI handlers for iOS in Maui/Platforms/Apple/
- [ ] T033 [P] [US2] Convert platform-specific renderers to MAUI handlers for Windows in Maui/Platforms/Windows/
- [ ] T034 [US2] Update platform-specific effects to MAUI effect system in Maui/Effects/
- [ ] T035 [US2] Migrate platform presenters from _skip folder to MAUI Shell navigation
- [ ] T036 [US2] Validate resource scaling works across all target platforms
- [ ] T037 [US2] Test platform-specific popup behavior with MAUI Community Toolkit
- [ ] T038 [US2] Update conditional compilation directives for MAUI platform detection
- [ ] T039 [US2] Verify platform-specific service registration in Maui/ServiceRegister.cs
- [ ] T040 [US2] Clean up migrated platform-specific code from Maui/_skip/Platforms/

**Checkpoint**: All three target platforms should have consistent functionality and platform-appropriate behavior

---

## Phase 5: User Story 3 - Development Team Productivity (Priority: P3)

**Goal**: Ensure development teams can efficiently build, debug, and deploy the MAUI application with updated tooling

**Independent Test**: Development team can complete common tasks with comparable or improved efficiency

### Tests for User Story 3

- [ ] T041 [P] [US3] Create build performance benchmark test in Brupper.Maui.Tests/US3/BuildPerformanceTests.cs
- [ ] T042 [P] [US3] Create debugging workflow validation test in Brupper.Maui.Tests/US3/DebuggingWorkflowTests.cs
- [ ] T043 [P] [US3] Create package consumer integration test in Brupper.Maui.Tests/US3/PackageIntegrationTests.cs

### Implementation for User Story 3

- [ ] T044 [P] [US3] Update package documentation in Maui/readme.md with MAUI-specific guidance
- [ ] T045 [P] [US3] Create migration guide documentation in specs/001-xamarin-to-maui-migration/migration-guide.md
- [ ] T046 [P] [US3] Document breaking changes and API differences in specs/001-xamarin-to-maui-migration/breaking-changes.md
- [ ] T047 [US3] Update build configuration for optimal MAUI development experience in Maui/Brupper.Maui.csproj
- [ ] T048 [US3] Create consumer quickstart samples demonstrating MAUI integration patterns
- [ ] T049 [US3] Establish CI/CD pipeline validation for all target platforms
- [ ] T050 [US3] Create debugging guidelines for MAUI-specific scenarios
- [ ] T051 [US3] Validate package publishing process for MAUI compatibility
- [ ] T052 [US3] Create developer onboarding checklist based on quickstart.md

**Checkpoint**: Development team should have all tools and documentation needed for efficient MAUI development

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and quality improvements across all user stories

- [ ] T053 [P] XML documentation compliance verification for all public APIs
- [ ] T054 [P] Static analysis compliance verification across all migrated code
- [ ] T055 [P] Performance regression testing against established baselines
- [ ] T056 [P] Memory leak validation for platform-specific handlers
- [ ] T057 [P] Cross-platform UI consistency validation
- [ ] T058 Code cleanup and removal of any remaining legacy references
- [ ] T059 Final validation of all success criteria from spec.md
- [ ] T060 Package versioning and release preparation
- [ ] T061 Consumer migration documentation finalization
- [ ] T062 Run complete quickstart.md validation end-to-end

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - User stories can proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Phase 6)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - Core migration, no dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Platform-specific work, may integrate with US1 patterns
- **User Story 3 (P3)**: Should start after US1 completion - Documentation requires migrated code examples

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Core component migration before platform-specific work
- Service layer before UI layer
- Validation before cleanup
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, US1 and US2 can start in parallel
- All tests for a user story marked [P] can run in parallel
- Platform-specific implementations marked [P] can run in parallel
- Documentation tasks in US3 marked [P] can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "MvvmCross component inventory test in Brupper.Maui.Tests/US1/ComponentInventoryTests.cs"
Task: "Navigation functionality test in Brupper.Maui.Tests/US1/NavigationPreservationTests.cs"
Task: "Popup functionality test in Brupper.Maui.Tests/US1/PopupFunctionalityTests.cs"
Task: "ViewModel binding test in Brupper.Maui.Tests/US1/ViewModelBindingTests.cs"

# Launch all component migrations together:
Task: "Migrate MvxBasePage to MAUI ContentPage pattern in Maui/Views/BasePage.cs"
Task: "Migrate MvxPopupPage to MAUI Community Toolkit Popup in Maui/Views/BasePopup.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (Core functionality preservation)
4. **STOP and VALIDATE**: Test User Story 1 independently - all MvvmCross components work with MAUI
5. Deploy/demo core MAUI functionality

### Incremental Delivery

1. Complete Setup + Foundational → Migration foundation ready
2. Add User Story 1 → Test independently → Core MAUI migration complete (MVP!)
3. Add User Story 2 → Test independently → Multi-platform validation complete
4. Add User Story 3 → Test independently → Full development team enablement
5. Each story adds value without breaking previous migration work

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (Core migration)
   - Developer B: User Story 2 (Platform compatibility) - can start basic work
   - Developer C: User Story 3 preparation (documentation framework)
3. US1 completes first, then US2 and US3 can accelerate with concrete examples

---

## Migration-Specific Notes

- Focus on MvvmCross to MAUI native pattern conversion
- Preserve all existing functionality during migration
- Use Shell Navigation + MVVM Community Toolkit as replacement framework
- Each migrated component should be tested independently
- Maintain performance baselines throughout migration
- Clean up _skip folder incrementally as components are successfully migrated
- Document all breaking changes for package consumers
- Validate on all target platforms (Android, iOS, Windows)

---

## Validation Checklist

### 🎯 MVP Success (User Story 1)
- [ ] All MvvmCross navigation works with Shell Navigation
- [ ] All popup functionality works with MAUI Community Toolkit
- [ ] All ViewModels work with MVVM Community Toolkit ObservableObject
- [ ] No Xamarin.Forms references in active code
- [ ] Core functionality identical to original

### ✅ Platform Ready (User Story 2)
- [ ] Builds successfully for Android, iOS, Windows
- [ ] Platform handlers replace all custom renderers
- [ ] Platform-specific behavior works correctly
- [ ] UI consistency across platforms validated

### 📚 Team Ready (User Story 3)
- [ ] Documentation updated for MAUI patterns
- [ ] Build/debug/deploy workflows validated
- [ ] Breaking changes documented with migration guidance
- [ ] Package consumers have clear upgrade path