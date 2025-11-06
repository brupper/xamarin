# Migration Tracking Spreadsheet

**Based on**: data-model.md entities
**Date**: November 6, 2025
**Format**: CSV with status tracking

## Summary

| Status | Count | Description |
|--------|-------|-------------|
| In Progress | 2 | Currently being worked on |
| Not Started | 20 | Planned for future phases |
| Identified | 5 | Legacy components identified |
| To Remove | 3 | Dependencies to eliminate |
| To Add | 2 | Dependencies to include |

## Detailed Tracking

| Entity Type | Entity Name | Migration Status | Priority | Complexity | Dependencies | Notes |
|-------------|-------------|------------------|----------|------------|--------------|-------|
| Mobile Application Package | Brupper.Maui | In Progress | P1 | High | None | Target frameworks already configured |
| Platform Implementation | Android | Not Started | P2 | Medium | Mobile Application Package | Convert renderers to handlers |
| Platform Implementation | iOS | Not Started | P2 | Medium | Mobile Application Package | Convert renderers to handlers |
| Platform Implementation | Windows | Not Started | P2 | Medium | Mobile Application Package | Convert renderers to handlers |
| MAUI Handler | Border Handler | Not Started | P2 | Low | Android Platform | Convert BorderPlatformEffect |
| MAUI Handler | Entry Handler | Not Started | P2 | Low | Android/iOS Platform | Convert EntryEffect |
| MAUI Handler | Picker Handler | Not Started | P2 | Low | Android/iOS Platform | Convert PickerEffect and PickerChangeColor |
| Legacy Renderer | BorderPlatformEffect | Identified | P2 | Low | None | Located in _skip/Platforms/Android/Effects/ |
| Legacy Renderer | EntryEffect (Android) | Identified | P2 | Low | None | Located in _skip/Platforms/Android/Effects/ |
| Legacy Renderer | EntryEffect (iOS) | Identified | P2 | Low | None | Located in _skip/Platforms/Ios/Effects/ |
| Legacy Renderer | PickerEffect | Identified | P2 | Low | None | Located in _skip/Platforms/Android/Effects/ |
| Legacy Renderer | PickerChangeColor | Identified | P2 | Low | None | Located in _skip/Platforms/Ios/Effects/ |
| Package Dependency | MvvmCross.Forms | To Remove | P1 | High | None | Replace with MAUI + Community Toolkit |
| Package Dependency | MvvmCross.Core | To Remove | P1 | High | None | Replace with Community Toolkit MVVM |
| Package Dependency | Rg.Plugins.Popup | To Remove | P1 | Medium | None | Replace with Community Toolkit Popup |
| Package Dependency | CommunityToolkit.Maui | To Add | P1 | Low | None | Already added to project |
| Package Dependency | CommunityToolkit.Mvvm | To Add | P1 | Low | None | Already added to project |
| Test Suite | Unit Tests | In Progress | P1 | Medium | None | xUnit framework configured |
| Test Suite | Integration Tests | Not Started | P2 | High | Unit Tests | Cross-platform testing needed |
| Test Suite | Platform Tests | Not Started | P2 | High | Unit Tests | Device-specific testing needed |
| Migration Task | MvxBasePage Migration | Not Started | P1 | Medium | None | Convert to MAUI ContentPage |
| Migration Task | MvxPopupPage Migration | Not Started | P1 | High | None | Convert to Community Toolkit Popup |
| Migration Task | PagePresenter Migration | Not Started | P1 | Medium | None | Replace with Shell Navigation |
| Migration Task | PopupPresentationAttribute Migration | Not Started | P1 | Low | None | Update for MAUI patterns |
| Migration Task | Platform Setup Migration | Not Started | P2 | Low | None | Move to MauiProgram.cs |
| API Change Documentation | Breaking Changes Doc | Not Started | P3 | Medium | None | Document for consumers |
| Platform Compatibility Matrix | Compatibility Matrix | Not Started | P2 | Low | None | Track feature support across platforms |

## Migration Phases

### Phase 1: Foundation (Current)
- ✅ Test project setup  
- ✅ Performance baseline established
- ✅ Legacy code audit completed
- ✅ Migration tracking created
- ✅ Package verification completed
- ✅ DI integration updated (T008 - AddBrupperMaui API)

### Phase 2: Core Migration (Next)
- ⚠️ Exclude _skip folder from build (T009 prerequisite)
- Mobile Application Package completion
- MvxBasePage and MvxPopupPage migration
- Navigation system replacement
- Dependency injection updates

### Phase 3: Platform Migration
- Handler conversions for all platforms
- Platform-specific setup migration
- Cross-platform compatibility validation

### Phase 4: Testing & Polish
- Integration and platform tests
- Documentation completion
- Performance validation

## Risk Mitigation

### High Priority Risks
1. **Navigation System Changes**: MvvmCross → Shell Navigation
   - **Mitigation**: Comprehensive testing of navigation flows
   - **Fallback**: Maintain MvvmCross compatibility layer during transition

2. **Popup Functionality**: Rg.Plugins.Popup → Community Toolkit
   - **Mitigation**: Test all popup scenarios before removing legacy code
   - **Fallback**: Keep Rg.Plugins.Popup as backup during migration

### Medium Priority Risks
3. **Platform Handler Conversion**: Effects → Handlers
   - **Mitigation**: Convert one platform at a time with testing
   - **Fallback**: Keep effects as backup during conversion

4. **Dependency Injection Changes**: MvvmCross → Microsoft.Extensions.DI
   - **Mitigation**: Update ServiceRegister.cs incrementally
   - **Fallback**: Maintain dual registration during transition

## Success Metrics

- [ ] All "To Remove" dependencies eliminated
- [ ] All "To Add" dependencies integrated
- [ ] Zero "Not Started" items remaining
- [ ] All platform handlers functional
- [ ] Performance meets baseline requirements
- [ ] All tests passing