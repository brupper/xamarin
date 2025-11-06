# Quickstart: Brupper.Maui Migration

**Audience**: Development team implementing the Xamarin.Forms to .NET MAUI migration  
**Prerequisites**: .NET 9 SDK, MAUI workload, Visual Studio 2022 17.8+  
**Estimated Time**: 2-3 days for complete migration

## Overview

This guide walks through migrating the Brupper.Maui package from any remaining Xamarin.Forms dependencies to full .NET MAUI compliance while preserving all functionality and maintaining backward compatibility where possible.

## Phase 1: Environment Setup (30 minutes)

### 1.1 Install Required Tools
```bash
# Install .NET 9 SDK
winget install Microsoft.DotNet.SDK.9

# Install MAUI workload
dotnet workload install maui

# Verify installation
dotnet workload list
```

### 1.2 Verify Project State
```bash
# Navigate to project directory
cd Maui/

# Check current target frameworks
dotnet list package --framework net9.0
dotnet list package --framework net9.0-android  
dotnet list package --framework net9.0-ios
```

### 1.3 Build Current State
```bash
# Build to establish baseline
dotnet build --configuration Release

# Run any existing tests
dotnet test --configuration Release
```

## Phase 2: Legacy Code Audit (2-4 hours)

### 2.1 Analyze _skip Folder Contents
```bash
# Find all Xamarin.Forms references
grep -r "Xamarin.Forms" Maui/_skip/ > legacy-references.txt

# Find custom renderers
find Maui/_skip/ -name "*Renderer*.cs" -o -name "*Effect*.cs"

# Review platform-specific setup code
ls -la Maui/_skip/Platforms/*/Setup*.cs
```

### 2.2 Categorize Legacy Components
Create a migration inventory:

| Component | Type | Priority | Action |
|-----------|------|----------|--------|
| Pages/Popups/*.xaml | UI Pages | High | Migrate to MAUI |
| Effects/*Effect.cs | Custom Effects | Medium | Convert to Handlers |
| Platforms/*/Setup.cs | Platform Setup | High | Update for MAUI |
| Custom Renderers | Platform UI | Medium | Convert to Handlers |

### 2.3 Identify Dependencies
```bash
# Check for deprecated packages
grep -i "xamarin" Maui/Brupper.Maui.csproj

# Review commented package references
grep "<!--.*Package.*Xamarin" Maui/Brupper.Maui.csproj
```

## Phase 3: Migration Execution (1-2 days)

### 3.1 Update Package References
```xml
<!-- Remove any remaining Xamarin.Forms packages -->
<!-- Update to MAUI equivalents in Brupper.Maui.csproj -->

<PackageReference Include="CommunityToolkit.Maui" />
<PackageReference Include="CommunityToolkit.Maui.Markup" />
<!-- Add any missing MAUI-specific packages -->
```

### 3.2 Migrate Legacy Pages
For each page in `_skip/Pages/`:

1. **Copy to main project structure**:
   ```bash
   cp Maui/_skip/Pages/Popups/QuestionPage.xaml Maui/Views/
   cp Maui/_skip/Pages/Popups/QuestionPage.xaml.cs Maui/Views/
   ```

2. **Update namespace declarations**:
   ```xml
   <!-- OLD: -->
   xmlns:ios="clr-namespace:Xamarin.Forms.PlatformConfiguration.iOSSpecific;assembly=Xamarin.Forms.Core"
   
   <!-- NEW: -->
   xmlns:ios="clr-namespace:Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;assembly=Microsoft.Maui.Controls"
   ```

3. **Update code-behind**:
   ```csharp
   // OLD:
   using Xamarin.Forms;
   
   // NEW: 
   using Microsoft.Maui.Controls;
   ```

### 3.3 Convert Effects to Handlers
For each effect in `_skip/Platforms/*/Effects/`:

1. **Create handler structure**:
   ```csharp
   // Example: EntryEffect -> EntryHandler extension
   public static class BrupperEntryHandler
   {
       public static void MapBrupperProperties(IEntryHandler handler, IEntry entry)
       {
           // Implementation
       }
   }
   ```

2. **Register handlers**:
   ```csharp
   // In platform-specific code
   Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(
       "BrupperEntry", BrupperEntryHandler.MapBrupperProperties);
   ```

### 3.4 Update Platform-Specific Code
For each platform setup in `_skip/Platforms/`:

1. **iOS Setup Migration**:
   ```csharp
   // OLD: Xamarin.Forms setup
   // Xamarin.Forms.Forms.Init();
   
   // NEW: MAUI setup (typically handled automatically)
   // Configure any custom platform services
   ```

2. **Android Setup Migration**:
   ```csharp
   // OLD: Xamarin.Forms initialization
   // Xamarin.Forms.Forms.Init(this, savedInstanceState);
   
   // NEW: MAUI platform services
   // Configure handlers in MauiProgram.cs
   ```

### 3.5 Update Service Registration
```csharp
// Update ServiceRegister.cs to use MAUI patterns
public static class ServiceRegister
{
    public static MauiAppBuilder AddBrupperMaui(this MauiAppBuilder builder)
    {
        // Configure handlers
        builder.ConfigureFonts(fonts => {
            // Font configuration
        });
        
        // Register services
        builder.Services.AddSingleton<IBrupperService, BrupperService>();
        
        return builder;
    }
}
```

## Phase 4: Testing and Validation (4-6 hours)

### 4.1 Create Test Project
```bash
# Create test project
dotnet new mstest -n Brupper.Maui.Tests
cd Brupper.Maui.Tests

# Add reference to main project
dotnet add reference ../Maui/Brupper.Maui.csproj

# Add MAUI testing packages
dotnet add package Microsoft.Maui.Controls
dotnet add package Moq
```

### 4.2 Platform Testing
```csharp
[TestClass]
public class PlatformCompatibilityTests
{
    [TestMethod]
    public void Android_HandlersRegistered()
    {
        // Test Android-specific functionality
    }
    
    [TestMethod] 
    public void iOS_HandlersRegistered()
    {
        // Test iOS-specific functionality
    }
    
    [TestMethod]
    public void Windows_HandlersRegistered() 
    {
        // Test Windows-specific functionality
    }
}
```

### 4.3 Performance Validation
```csharp
[TestClass]
public class PerformanceTests
{
    [TestMethod]
    public void ServiceRegistration_PerformanceBaseline()
    {
        // Measure startup time impact
        var stopwatch = Stopwatch.StartNew();
        // Test service registration
        stopwatch.Stop();
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100);
    }
}
```

### 4.4 API Compatibility Testing
```csharp
[TestClass]
public class ApiCompatibilityTests
{
    [TestMethod]
    public void PublicApi_BackwardCompatible()
    {
        // Verify all expected public APIs are available
        var assembly = typeof(ServiceRegister).Assembly;
        var publicTypes = assembly.GetExportedTypes();
        // Assert expected types exist
    }
}
```

## Phase 5: Documentation and Cleanup (2-3 hours)

### 5.1 Update Package Documentation
```markdown
# Update readme.md with:
- MAUI-specific initialization instructions
- Breaking changes from Xamarin.Forms
- Migration guide for consumers
- Platform-specific considerations
```

### 5.2 Clean Up Legacy Code
```bash
# Review _skip folder - keep only reference material
# Remove successfully migrated components
rm -rf Maui/_skip/Pages/Popups/  # If fully migrated
```

### 5.3 Update Build Configuration
```xml
<!-- Ensure all target frameworks build successfully -->
<PropertyGroup>
    <TargetFrameworks>net9.0;net9.0-android;net9.0-ios;net9.0-windows</TargetFrameworks>
    <!-- Add Windows target if needed -->
</PropertyGroup>
```

## Validation Checklist

### ✅ Migration Complete Criteria
- [ ] All Xamarin.Forms references removed from active code
- [ ] Package builds successfully for all target frameworks
- [ ] All platform-specific handlers registered and tested
- [ ] Performance benchmarks show no significant regression
- [ ] Test coverage maintains or improves from baseline
- [ ] Documentation updated with migration guidance
- [ ] Breaking changes clearly documented
- [ ] Consumer migration path defined

### ✅ Quality Gates Passed
- [ ] Static analysis passes with zero warnings
- [ ] All public APIs have XML documentation
- [ ] Platform-specific code properly isolated
- [ ] Dependency injection patterns follow MAUI conventions
- [ ] Error handling maintains consistency

## Common Issues and Solutions

### Issue: Custom Renderer Not Working
**Solution**: Convert to MAUI Handler pattern
```csharp
// OLD: Renderer
public class CustomEntryRenderer : EntryRenderer
// NEW: Handler extension
public static class CustomEntryHandler
```

### Issue: Platform-Specific Code Not Executing
**Solution**: Verify conditional compilation directives
```csharp
#if ANDROID
    // Android-specific code
#elif IOS  
    // iOS-specific code
#endif
```

### Issue: Missing Platform Dependencies
**Solution**: Add platform-specific package references
```xml
<ItemGroup Condition="$(TargetFramework.StartsWith('net9.0-android'))">
    <PackageReference Include="AndroidSpecificPackage" />
</ItemGroup>
```

## Next Steps

After completing this migration:
1. **Monitor performance** in real-world usage
2. **Gather feedback** from package consumers  
3. **Plan feature additions** leveraging MAUI capabilities
4. **Consider Windows platform** enhancements
5. **Update CI/CD pipelines** for MAUI build requirements

## Support and Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [Xamarin.Forms to MAUI Migration Guide](https://docs.microsoft.com/dotnet/maui/migration/)
- [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui)
- [Platform Handler Documentation](https://docs.microsoft.com/dotnet/maui/user-interface/handlers/)