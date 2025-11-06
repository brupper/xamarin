# Routing Migration Guide

## Overview

This document explains the migration from MvvmCross presentation attributes to MAUI Shell routing.

## Migration Summary

| Legacy Approach | MAUI Approach |
|----------------|---------------|
| `[PopupPresentation]` attribute | `Routing.RegisterRoute()` + Shell navigation |
| `[MvxContentPagePresentation]` attribute | `Routing.RegisterRoute()` + Shell navigation |
| Declarative (attributes on classes) | Imperative (registration in code) |
| Presenter-based | Shell-based |

## Before (MvvmCross)

### Popup Example

```csharp
using Brupper.Forms.Attributes;

namespace Brupper.Forms.Pages.Popups
{
    [PopupPresentation(Animated = true, WrapInNavigationPage = false)]
    public partial class AlertPopup : MvxPopupPage<AlertViewModel>
    {
        public AlertPopup()
        {
            InitializeComponent();
        }
    }
}
```

### Navigation

```csharp
// In ViewModel
await navigationService.Navigate<AlertViewModel, AlertViewModelParameter>(param);
```

## After (MAUI)

### 1. Register Route (One-time setup)

**In `MauiProgram.cs` or during app startup:**

```csharp
using Brupper.Maui.Services;

// Option 1: Use the centralized registration
RouteRegistration.RegisterRoutes(loggerFactory);

// Option 2: Register individual routes
Routing.RegisterRoute(RouteRegistration.Routes.AlertPopup, typeof(AlertPopup));
```

### 2. Create Popup (No attribute needed)

```csharp
namespace Brupper.Maui.Views.Popups;

public partial class AlertPopup : BasePopup<AlertViewModel>
{
    public AlertPopup()
    {
        InitializeComponent();
        CloseWhenBackgroundIsClicked = false; // Previously in [PopupPresentation]
    }
}
```

### 3. Navigation

**Using the NavigationService (recommended):**

```csharp
// In ViewModel - showing a popup
var result = await NavigationService.ShowPopupAsync<AlertPopup, AlertViewModel, AlertViewModelParameter, SimpleStateHolder>(param);

// Or for simple popup without result
await NavigationService.ShowPopupAsync<AlertPopup, AlertViewModel>();
```

**Using route constants (type-safe):**

```csharp
// Navigate by route
await NavigationService.NavigateToAsync(RouteRegistration.Routes.AlertPopup, parameters);
```

## Popup Configuration Migration

### Animated Property

**Before:**
```csharp
[PopupPresentation(Animated = true)]
```

**After:**
```csharp
// Set in popup constructor or when showing
public AlertPopup()
{
    // Animation is handled by CommunityToolkit.Maui.Views.Popup
    // No explicit configuration needed - animates by default
}
```

### CloseWhenBackgroundIsClicked

**Before:**
```csharp
// Set on MvxPopupPage base class
CloseWhenBackgroundIsClicked = false;
```

**After:**
```csharp
// Same - set in BasePopup constructor
public AlertPopup()
{
    InitializeComponent();
    CloseWhenBackgroundIsClicked = false;
}
```

### WrapInNavigationPage

**Before:**
```csharp
[PopupPresentation(WrapInNavigationPage = false)]
```

**After:**
```csharp
// Not applicable for popups
// Popups in MAUI are overlays and don't participate in navigation stack
```

## Page Navigation Migration

### Before (MvvmCross)

```csharp
[MvxContentPagePresentation]
public partial class EditorPage : MvxBasePage<EditorViewModel>
{
}

// Navigation
await navigationService.Navigate<EditorViewModel, EditorParam>(param);
```

### After (MAUI)

```csharp
// 1. Register route
Routing.RegisterRoute(RouteRegistration.Routes.EditorPage, typeof(EditorPage));

// 2. Create page (no attribute)
public partial class EditorPage : BasePage<EditorViewModel>
{
}

// 3. Navigate
await NavigationService.NavigateToAsync<EditorPage, EditorViewModel, EditorParam>(param);
// Or by route
await NavigationService.NavigateToAsync(RouteRegistration.Routes.EditorPage, parameters);
```

## Route Naming Conventions

### Recommended Pattern

- **Popups:** `popup/{name}` (e.g., `popup/alert`, `popup/question`)
- **Pages:** `page/{name}` (e.g., `page/editor`, `page/selector`)
- **Nested:** `parent/child` (e.g., `settings/profile`, `items/details`)

### Constants Usage

Always use `RouteRegistration.Routes` constants instead of magic strings:

```csharp
// ✅ Good - type-safe, refactorable
await NavigationService.NavigateToAsync(RouteRegistration.Routes.AlertPopup);

// ❌ Bad - magic string, typo-prone
await NavigationService.NavigateToAsync("popup/alert");
```

## Migration Checklist

For each page/popup being migrated:

- [ ] Remove `[PopupPresentation]` or `[MvxContentPagePresentation]` attribute
- [ ] Change base class from `MvxPopupPage<T>` to `BasePopup<T>` (or `MvxBasePage<T>` to `BasePage<T>`)
- [ ] Add route registration in `RouteRegistration.RegisterRoutes()`
- [ ] Add route constant to `RouteRegistration.Routes`
- [ ] Update navigation calls to use new `NavigationService`
- [ ] Move configuration (Animated, CloseWhenBackgroundIsClicked) to constructor
- [ ] Test navigation works correctly

## Benefits of Shell Routing

1. **Centralized:** All routes visible in one place (`RouteRegistration.cs`)
2. **Type-safe:** Use constants instead of magic strings
3. **Flexible:** Can change routes without modifying view code
4. **Standard:** Uses MAUI's built-in Shell navigation system
5. **Deep Linking:** Shell routes support URI-based navigation
6. **Testing:** Easier to mock and test navigation

## Example: Complete Migration

### Before (MvvmCross)

```csharp
// AlertPopup.xaml.cs
[PopupPresentation(Animated = true, WrapInNavigationPage = false)]
public partial class AlertPopup : MvxPopupPage<AlertViewModel>
{
    public AlertPopup()
    {
        InitializeComponent();
    }
}

// In some ViewModel
await mvxNavigationService.Navigate<AlertViewModel, AlertParam>(param);
```

### After (MAUI)

```csharp
// RouteRegistration.cs
public static void RegisterRoutes(ILoggerFactory? loggerFactory = null)
{
    RegisterRoute(Routes.AlertPopup, typeof(AlertPopup));
}

public static class Routes
{
    public const string AlertPopup = "popup/alert";
}

// AlertPopup.xaml.cs (no attribute)
public partial class AlertPopup : BasePopup<AlertViewModel>
{
    public AlertPopup()
    {
        InitializeComponent();
        CloseWhenBackgroundIsClicked = false;
    }
}

// In some ViewModel (using new NavigationService)
var result = await NavigationService.ShowPopupAsync<AlertPopup, AlertViewModel, AlertParam, AlertResult>(param);
```

## Additional Resources

- [MAUI Shell Navigation](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation)
- [CommunityToolkit.Maui Popup](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup)
- Project file: `Maui/Services/RouteRegistration.cs`
- Base classes: `Maui/Views/BasePage.cs`, `Maui/Views/BasePopup.cs`
