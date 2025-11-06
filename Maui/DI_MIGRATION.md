# Dependency Injection Migration Guide

## Overview

This guide documents the migration from **MvvmCross IoC** (`Mvx.IoCProvider`) to **Microsoft.Extensions.DependencyInjection** for the Brupper MAUI project.

## Table of Contents

- [Why Migrate?](#why-migrate)
- [Service Registration](#service-registration)
- [Service Resolution](#service-resolution)
- [Migration Patterns](#migration-patterns)
- [Registered Services](#registered-services)
- [Best Practices](#best-practices)

## Why Migrate?

**Benefits of Microsoft.Extensions.DependencyInjection:**

1. **Native MAUI Integration**: MAUI uses Microsoft.Extensions.DependencyInjection by default
2. **Better Performance**: More efficient service resolution
3. **Standard .NET Pattern**: Used across ASP.NET Core, Generic Host, and MAUI
4. **Better Tooling**: IntelliSense, analyzers, and debugging support
5. **Scoped Services**: Support for scoped service lifetimes (not just singleton/transient)
6. **Constructor Injection**: First-class support for dependency injection via constructors

## Service Registration

### MauiProgram.cs Setup

```csharp
using Brupper;
using CommunityToolkit.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseBrupperMaui()  // Registers all Brupper services
            .UseMauiCommunityToolkit();

        // Register your ViewModels
        builder.Services.AddViewModel<MainViewModel>();
        builder.Services.AddViewModel<DetailViewModel>();
        
        // Register your Views
        builder.Services.AddView<MainPage>();
        builder.Services.AddView<DetailPage>();
        
        // Or register both together
        builder.Services.AddViewWithViewModel<SettingsPage, SettingsViewModel>();

        return builder.Build();
    }
}
```

### Service Lifetimes

**Transient** - New instance every time:
```csharp
services.AddTransient<IMyService, MyService>();
services.AddViewModel<MyViewModel>(); // ViewModels are transient by default
services.AddView<MyPage>(); // Views are transient by default
```

**Singleton** - One instance for the app lifetime:
```csharp
services.AddSingleton<IMyService, MyService>();
services.AddViewModelSingleton<MyViewModel>(); // For app-level state
```

**Scoped** - One instance per scope (useful for web apps, less common in MAUI):
```csharp
services.AddScoped<IMyService, MyService>();
```

## Service Resolution

### OLD: MvvmCross IoC

```csharp
// ❌ OLD - MvvmCross pattern
using MvvmCross;

var service = Mvx.IoCProvider.Resolve<IMyService>();
var serviceSingleton = Mvx.IoCProvider.GetSingleton<IMyService>();
Mvx.IoCProvider.TryResolve<IMyService>(out var service);
```

### NEW: Constructor Injection (Preferred)

```csharp
// ✅ NEW - Constructor injection (PREFERRED)
public class MyViewModel : ViewModelBase
{
    private readonly IMyService _myService;
    
    public MyViewModel(
        ILogger logger,
        INavigationService navigationService,
        IMyService myService)  // ← Injected automatically
        : base(logger, navigationService)
    {
        _myService = myService;
    }
}
```

### NEW: Service Locator Pattern (Use Sparingly)

```csharp
// ⚠️ NEW - Service locator (use only when constructor injection isn't possible)
using Brupper.Maui.Services;

var service = ServiceProvider.GetService<IMyService>();
```

**Important**: Prefer constructor injection over service locator pattern. Only use `ServiceProvider.GetService<T>()` when:
- You're in legacy code that can't easily be refactored
- You need to resolve services dynamically at runtime
- You're in a static context where DI isn't available

## Migration Patterns

### Pattern 1: ViewModel with Dependencies

**Before (MvvmCross):**
```csharp
public class ProductListViewModel : MvxViewModel
{
    private readonly IProductRepository _repository;
    
    public ProductListViewModel()
    {
        _repository = Mvx.IoCProvider.Resolve<IProductRepository>();
    }
}
```

**After (MAUI DI):**
```csharp
public class ProductListViewModel : ViewModelBase
{
    private readonly IProductRepository _repository;
    
    public ProductListViewModel(
        ILogger logger,
        INavigationService navigationService,
        IProductRepository repository)
        : base(logger, navigationService)
    {
        _repository = repository;
    }
}
```

### Pattern 2: Service Resolution in Methods

**Before (MvvmCross):**
```csharp
public async Task LoadDataAsync()
{
    using (var repository = Mvx.IoCProvider.Resolve<IProductRepository>())
    {
        var products = await repository.GetAllAsync();
        // ...
    }
}
```

**After (MAUI DI):**
```csharp
private readonly IProductRepository _repository;

public ProductListViewModel(
    ILogger logger,
    INavigationService navigationService,
    IProductRepository repository)
    : base(logger, navigationService)
{
    _repository = repository;
}

public async Task LoadDataAsync()
{
    var products = await _repository.GetAllAsync();
    // ...
}
```

### Pattern 3: Optional Dependencies

**Before (MvvmCross):**
```csharp
IMvxTextProvider? textProvider = null;
if (Mvx.IoCProvider.TryResolve<IMvxTextProvider>(out var provider))
{
    textProvider = provider;
}
```

**After (MAUI DI):**
```csharp
public class MyViewModel : ViewModelBase
{
    private readonly ILocalizationService? _localization;
    
    public MyViewModel(
        ILogger logger,
        INavigationService navigationService,
        ILocalizationService? localization = null) // ← Optional parameter
        : base(logger, navigationService)
    {
        _localization = localization;
    }
}
```

### Pattern 4: Localization/Text Provider

**Before (MvvmCross):**
```csharp
var textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>();
var text = textProvider.GetText("MyResourceKey");
```

**After (MAUI DI):**
```csharp
private readonly ILocalizationService _localization;

public MyViewModel(
    ILogger logger,
    INavigationService navigationService,
    ILocalizationService localization)
    : base(logger, navigationService)
{
    _localization = localization;
}

public void DoSomething()
{
    var text = _localization.GetText("MyResourceKey");
    var formatted = _localization.GetText("WelcomeMessage", userName);
}
```

## Registered Services

### Core Services (All Platforms)

| Service Interface | Implementation | Lifetime | Description |
|------------------|----------------|----------|-------------|
| `INavigationService` | `NavigationService` | Singleton | Shell-based navigation |
| `IConnectivity` | `ConnectivityService` | Singleton | Network connectivity |
| `IFileSystem` | `FileSystemService` | Singleton | File system access |
| `ILocalizationService` | `LocalizationService` | Singleton | Text localization |
| `ILogger` | Brupper `Logger.Current` | Singleton | Brupper logging |
| `ILogger<T>` | MS Extensions Logging | Transient | Type-specific MS logger |
| `ILoggerFactory` | MS Extensions Logging | Singleton | Logger factory |

### Platform-Specific Services

| Service Interface | Platform Implementation | Lifetime | Description |
|------------------|------------------------|----------|-------------|
| `IPlatformInformationService` | Android/iOS/Windows specific | Singleton | Platform info |
| `IPermissionHelper` | Android/iOS/Windows specific | Singleton | Permission handling |
| `IImageResizer` | Android/iOS/Windows specific | Singleton | Image processing |
| `IOutputRendererServices` | Android/iOS/Windows specific | Singleton | Rendering services |

### Diagnostics Services

| Service Interface | Implementation | Lifetime | Description |
|------------------|----------------|----------|-------------|
| `IDiagnosticsPlatformInformationProvider` | → `IPlatformInformationService` | Singleton | Alias for platform info |
| `IDiagnosticsStorage` | `FormsStorage` | Singleton | Diagnostics storage |
| `FormsLogger` | `FormsLogger` | Singleton | Forms-specific logger |

## Best Practices

### ✅ DO

1. **Use Constructor Injection**
   ```csharp
   public MyViewModel(ILogger logger, IMyService service)
       : base(logger, navigationService)
   {
       _service = service;
   }
   ```

2. **Register Services Early** in `MauiProgram.cs`:
   ```csharp
   builder.Services.AddSingleton<IMyService, MyService>();
   ```

3. **Use Appropriate Lifetimes**:
   - Transient for ViewModels (new instance per navigation)
   - Singleton for stateless services
   - Scoped for per-request state (rare in MAUI)

4. **Validate Registration** with tests:
   ```csharp
   [Fact]
   public void Service_Should_Be_Resolvable()
   {
       var provider = services.BuildServiceProvider();
       var service = provider.GetService<IMyService>();
       Assert.NotNull(service);
   }
   ```

### ❌ DON'T

1. **Don't Use Service Locator Pattern** unnecessarily:
   ```csharp
   // ❌ BAD - service locator
   var service = ServiceProvider.GetService<IMyService>();
   
   // ✅ GOOD - constructor injection
   public MyClass(IMyService service) { ... }
   ```

2. **Don't Store Service Provider**:
   ```csharp
   // ❌ BAD
   private readonly IServiceProvider _serviceProvider;
   
   // ✅ GOOD
   private readonly IMyService _myService;
   ```

3. **Don't Resolve Services in Constructors** (circular dependency risk):
   ```csharp
   // ❌ BAD
   public MyViewModel(IServiceProvider provider)
   {
       _service = provider.GetService<IMyService>();
   }
   
   // ✅ GOOD
   public MyViewModel(IMyService service)
   {
       _service = service;
   }
   ```

4. **Don't Mix MvvmCross and MS DI**:
   ```csharp
   // ❌ BAD - mixing patterns
   public MyViewModel(ILogger logger)
   {
       _repository = Mvx.IoCProvider.Resolve<IRepository>();
   }
   ```

## Testing with DI

```csharp
[Fact]
public void ViewModel_Should_Use_Injected_Service()
{
    // Arrange
    var mockService = new Mock<IMyService>();
    var logger = Mock.Of<ILogger>();
    var navigation = Mock.Of<INavigationService>();
    
    // Act
    var viewModel = new MyViewModel(logger, navigation, mockService.Object);
    
    // Assert
    Assert.NotNull(viewModel);
    // ... test behavior
}
```

## Common Issues and Solutions

### Issue: Service not found

**Error**: `Unable to resolve service for type 'IMyService'`

**Solution**: Register the service in `MauiProgram.cs`:
```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

### Issue: Circular dependency

**Error**: `A circular dependency was detected`

**Solution**: Refactor to remove circular dependency or use a factory pattern:
```csharp
services.AddSingleton<IMyServiceFactory, MyServiceFactory>();
```

### Issue: Wrong lifetime

**Problem**: Singleton when should be transient (or vice versa)

**Solution**: Review service lifetime requirements:
- Transient: New instance each time
- Singleton: One instance for app lifetime
- Scoped: One instance per scope

## Migration Checklist

- [x] Register all services in `MauiProgram.cs` or `ServiceRegister.cs`
- [x] Replace `Mvx.IoCProvider.Resolve<T>()` with constructor injection
- [x] Replace `Mvx.IoCProvider.GetSingleton<T>()` with constructor injection
- [x] Replace `IMvxTextProvider` with `ILocalizationService`
- [x] Update all ViewModels to use constructor injection
- [x] Remove all `using MvvmCross` statements from active code
- [x] Add service registration tests
- [x] Verify all services resolve correctly

## Additional Resources

- [Microsoft.Extensions.DependencyInjection Documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [MAUI Dependency Injection](https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/dependency-injection)
- [Service Lifetimes](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
