# Package API Contracts

## Public API Surface

The Brupper.Maui package exposes the following public API contracts that must be maintained during migration:

### ServiceRegister Contract
```csharp
public static class ServiceRegister
{
    /// <summary>
    /// Registers Brupper MAUI services with the dependency injection container
    /// </summary>
    /// <param name="services">The service collection to register services with</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBrupperMaui(this IServiceCollection services);
    
    /// <summary>
    /// Configures Brupper MAUI services with custom options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Configuration action for Brupper options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBrupperMaui<TOptions>(
        this IServiceCollection services, 
        Action<TOptions> configureOptions) where TOptions : class;
}
```

### SystemInfo Contract
```csharp
public static class SystemInfo
{
    /// <summary>
    /// Gets the current platform information
    /// </summary>
    public static PlatformInfo CurrentPlatform { get; }
    
    /// <summary>
    /// Gets device-specific information
    /// </summary>
    public static DeviceInfo Device { get; }
    
    /// <summary>
    /// Determines if the application is running on a specific platform
    /// </summary>
    /// <param name="platform">The platform to check</param>
    /// <returns>True if running on the specified platform</returns>
    public static bool IsRunningOn(Platform platform);
}
```

### Converter Contracts
All value converters in the Converters/ folder must implement:
```csharp
public interface IBrupperConverter : IValueConverter
{
    /// <summary>
    /// Gets the converter description for debugging
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Indicates if the converter supports two-way binding
    /// </summary>
    bool SupportsTwoWayBinding { get; }
}
```

### Platform Handler Contracts
Platform-specific handlers must implement:
```csharp
public interface IPlatformHandler<TNative, TVirtual> 
    where TNative : class 
    where TVirtual : IView
{
    /// <summary>
    /// Creates the native platform view
    /// </summary>
    TNative CreatePlatformView();
    
    /// <summary>
    /// Connects the virtual view to the native view
    /// </summary>
    void ConnectHandler(TVirtual virtualView);
    
    /// <summary>
    /// Disconnects the handler and cleans up resources
    /// </summary>
    void DisconnectHandler();
}
```

### Extension Method Contracts
Extension methods must follow consistent patterns:
```csharp
public static class ViewExtensions
{
    /// <summary>
    /// Applies Brupper-specific styling to any view
    /// </summary>
    /// <typeparam name="T">The view type</typeparam>
    /// <param name="view">The view to style</param>
    /// <param name="style">The style to apply</param>
    /// <returns>The view for method chaining</returns>
    public static T ApplyBrupperStyle<T>(this T view, BrupperStyle style) where T : View;
}
```

## Breaking Changes from Migration

### Removed Xamarin.Forms Dependencies
- All `using Xamarin.Forms` statements removed
- Replaced with `using Microsoft.Maui.Controls`
- Platform-specific using statements updated

### Renderer to Handler Migration
- Custom renderers converted to MAUI handlers
- Handler registration differs from renderer registration
- Platform-specific handler implementations required

### Effect System Changes
- MAUI effects API differs from Xamarin.Forms
- Some effects may have built-in MAUI alternatives
- Platform-specific effect implementations updated

## Migration Contract Validation

### Pre-Migration Checklist
- [ ] All public APIs documented
- [ ] Platform-specific code isolated
- [ ] Dependencies are MAUI-compatible
- [ ] Test coverage established

### Post-Migration Validation
- [ ] All public APIs preserved or documented as breaking changes
- [ ] Platform implementations tested on target devices
- [ ] Performance benchmarks validate no regression
- [ ] Package builds successfully for all target frameworks

### Consumer Migration Guidance
Applications consuming Brupper.Maui will need to:

1. **Update project file** to target MAUI frameworks
2. **Update using statements** from Xamarin.Forms to Microsoft.Maui.Controls
3. **Review custom renderers** if extending Brupper components
4. **Update initialization code** for MAUI patterns
5. **Test platform-specific functionality** thoroughly

### Version Compatibility Matrix
| Brupper.Maui Version | .NET Version | MAUI Version | Breaking Changes |
|---------------------|--------------|--------------|------------------|
| 9.0.x               | .NET 9       | MAUI 9.0     | Xamarin.Forms removal |
| 8.0.x               | .NET 8       | MAUI 8.0     | N/A (Previous) |

## Testing Contracts

### Unit Testing Requirements
- All public methods must have unit tests
- Platform abstractions must be testable
- Dependency injection patterns must be validated

### Integration Testing Requirements  
- Platform-specific handlers tested on actual devices
- Cross-platform consistency validated
- Performance impact measured and documented

### Platform Testing Requirements
- Android: API level 21+ (as per MAUI requirements)
- iOS: iOS 11+ (as per MAUI requirements)  
- Windows: Windows 10 version 1809+ (as per MAUI requirements)