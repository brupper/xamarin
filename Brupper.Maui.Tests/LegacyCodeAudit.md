# Legacy Code Audit: Maui/_skip/ Folder

**Date**: November 6, 2025
**Total Files**: 25 C# files
**Status**: Ready for migration to MAUI patterns

## Executive Summary

The `_skip` folder contains legacy MvvmCross code that needs to be migrated to MAUI native patterns. The migration involves converting MvvmCross navigation, popup functionality, and MVVM patterns to MAUI Shell Navigation, Community Toolkit Popup, and MVVM Community Toolkit.

## File Inventory by Category

### Pages (UI Components) - 6 files
- `Pages/MvxBasePage.cs` - Base page with back button handling
- `Pages/MvxPopupPage.cs` - Popup page with MvvmCross binding
- `Pages/Popups/AlertPage.xaml.cs` - Alert popup implementation
- `Pages/Popups/InformationPage.xaml.cs` - Information popup implementation
- `Pages/Popups/QuestionPage.xaml.cs` - Question popup implementation

### Attributes (Presentation) - 1 file
- `Attributes/PopupPresentationAttribute.cs` - Custom popup presentation attribute

### Presenters (Navigation) - 1 file
- `Presenters/PagePresenter.cs` - Custom page presenter for popups

### Platforms (Platform-specific) - 8 files
- `Platforms/Android/Core/Setup.android.cs` - Android platform setup
- `Platforms/Android/Effects/BorderPlatformEffect.android.cs` - Border effect
- `Platforms/Android/Effects/EntryEffect.android.cs` - Entry effect
- `Platforms/Android/Effects/PickerEffect.android.cs` - Picker effect
- `Platforms/Ios/Core/Setup.ios.cs` - iOS platform setup
- `Platforms/Ios/Effects/EntryEffect.ios.cs` - Entry effect
- `Platforms/Ios/Effects/PickerChangeColor.ios.cs` - Picker color effect
- `Platforms/Windows/Core/Setup.windows.cs` - Windows platform setup

### ViewModels (Business Logic) - 9 files
- `ViewModels/Abstraction/_AEditorViewModel.cs` - Editor base ViewModel
- `ViewModels/Abstraction/_AItemsSearchViewModel.cs` - Search ViewModel
- `ViewModels/Abstraction/_AItemsViewModel.cs` - Items ViewModel
- `ViewModels/Abstraction/_ASelectorViewModel.cs` - Selector ViewModel
- `ViewModels/Abstractions/_AViewModelBase.cs` - Base ViewModel

## Migration Priority Analysis

### High Priority (Core Functionality)
1. **MvxBasePage.cs** → MAUI ContentPage with Shell navigation
2. **MvxPopupPage.cs** → MAUI Community Toolkit Popup
3. **PagePresenter.cs** → MAUI Shell navigation system
4. **PopupPresentationAttribute.cs** → MAUI popup attributes

### Medium Priority (Platform Effects)
5. **Platform Effects** → MAUI Handlers
   - BorderPlatformEffect → Border handler
   - EntryEffect → Entry handler
   - PickerEffect → Picker handler
   - PickerChangeColor → Picker handler

### Low Priority (Platform Setup)
6. **Platform Setup files** → MAUI platform services
   - Android Setup → MauiProgram.cs Android configuration
   - iOS Setup → MauiProgram.cs iOS configuration
   - Windows Setup → MauiProgram.cs Windows configuration

### Future (ViewModels)
7. **ViewModel abstractions** → Migrate after core UI migration complete

## Key Migration Patterns

### Navigation Migration
```csharp
// FROM: MvvmCross Navigation
public class MvxBasePage<TViewModel> : MvxContentPage<TViewModel>

// TO: MAUI Shell Navigation
public class BasePage<TViewModel> : ContentPage where TViewModel : ObservableObject
```

### Popup Migration
```csharp
// FROM: MvvmCross + Rg.Plugins.Popup
[PopupPresentation(Animated = true)]
public class MvxPopupPage<TViewModel> : PopupPage, IMvxPage<TViewModel>

// TO: MAUI Community Toolkit Popup
public class BasePopup<TViewModel> : Popup where TViewModel : ObservableObject
```

### ViewModel Migration
```csharp
// FROM: MvvmCross ViewModel
public class MyViewModel : MvxViewModel

// TO: MVVM Community Toolkit
public class MyViewModel : ObservableObject
```

## Dependencies to Remove

- MvvmCross.Forms
- MvvmCross.Core
- Rg.Plugins.Popup
- All Xamarin.Forms references

## Dependencies to Add

- CommunityToolkit.Maui
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection

## Migration Checklist

### Phase 1: Core UI Components
- [ ] Migrate MvxBasePage to MAUI ContentPage
- [ ] Migrate MvxPopupPage to MAUI Community Toolkit Popup
- [ ] Update popup presentation attributes
- [ ] Replace MvvmCross navigation with Shell navigation

### Phase 2: Platform Effects
- [ ] Convert Android effects to MAUI handlers
- [ ] Convert iOS effects to MAUI handlers
- [ ] Convert Windows effects to MAUI handlers
- [ ] Update platform-specific registrations

### Phase 3: Platform Setup
- [ ] Migrate Android setup to MauiProgram.cs
- [ ] Migrate iOS setup to MauiProgram.cs
- [ ] Migrate Windows setup to MauiProgram.cs
- [ ] Update dependency injection patterns

### Phase 4: ViewModels
- [ ] Migrate ViewModel base classes to ObservableObject
- [ ] Update navigation commands
- [ ] Update lifecycle event handling

## Risk Assessment

### Low Risk
- Platform effects (straightforward handler conversion)
- Platform setup (configuration migration)

### Medium Risk
- Navigation patterns (Shell vs MvvmCross differences)
- Popup lifecycle (Community Toolkit vs Rg.Plugins.Popup)

### High Risk
- Complex ViewModel interactions
- Custom platform integrations

## Success Criteria

- [ ] All 25 legacy files successfully migrated or removed
- [ ] No MvvmCross references in active codebase
- [ ] All platform effects converted to handlers
- [ ] Navigation and popup functionality preserved
- [ ] Performance meets or exceeds baseline
- [ ] All tests pass with MAUI patterns