# Migration Validation Framework

This infrastructure provides custom assertion helpers and utilities for validating the Xamarin.Forms to MAUI migration.

## Components

### MauiAssert
Custom assertion helpers for MAUI-specific validation:

```csharp
// Validate MAUI controls
MauiAssert.IsValidMauiControl(element);
MauiAssert.HasValidHandler<MyHandler>(element);

// Validate bindable properties
MauiAssert.HasBindableProperty(typeof(MyControl), "MyProperty", typeof(string), "default");

// Validate colors and layout
MauiAssert.ColorEqual(Colors.Red, control.BackgroundColor);
MauiAssert.HasValidLayout(view);

// Validate migration
MauiAssert.DependencyPropertyMigrated(typeof(XamarinControl), typeof(MauiControl), "PropertyName");
MauiAssert.NamespaceMigrated(typeof(MyControl), "Microsoft.Maui.Controls");

// Validate data binding
MauiAssert.SupportsDataBinding(control, MyControl.PropertyProperty);
```

### HandlerMigrationAssert
Specialized assertions for renderer-to-handler migration:

```csharp
// Validate renderer replacement
HandlerMigrationAssert.RendererReplacedByHandler(
    "MyRenderer",
    typeof(MyHandler),
    typeof(MyView));

// Validate handler structure
HandlerMigrationAssert.HasValidMapper<MyView>(handler.PropertyMapper);
HandlerMigrationAssert.NativeViewIsCorrectType<Android.Views.View>(handler);

// Validate migration patterns
HandlerMigrationAssert.OnElementChangedMigrated(typeof(MyHandler));
HandlerMigrationAssert.OnElementPropertyChangedMigrated(typeof(MyHandler));
HandlerMigrationAssert.DisposePatternsPreserved(typeof(MyHandler));

// Validate platform implementations
HandlerMigrationAssert.HasAllPlatformImplementations(
    typeof(MyHandler),
    "Android", "iOS", "Windows");
```

### PlatformTestHelpers
Utilities for platform-specific testing:

```csharp
// Platform detection
var platform = PlatformTestHelpers.GetCurrentPlatform(); // "Android", "iOS", "Windows"
if (PlatformTestHelpers.IsAndroid()) { /* Android-specific */ }

// Conditional test execution
PlatformTestHelpers.RequirePlatform("Android"); // Skips test if not on Android

// Handler validation
var isValid = PlatformTestHelpers.IsValidHandlerForPlatform(typeof(MyHandler));
var nativeType = PlatformTestHelpers.GetNativeViewType("Button");

// Test element creation
var element = PlatformTestHelpers.CreateTestElement<Button>();
```

## Usage Patterns

### Basic Control Validation
```csharp
[Fact]
public void MyControl_IsValidMauiControl()
{
    // Arrange
    var control = new MyControl();
    
    // Assert
    MauiAssert.IsValidMauiControl(control);
    MauiAssert.HasValidLayout(control);
    MauiAssert.ImplementsPropertyChanged(control);
}
```

### Handler Migration Test
```csharp
[Fact]
public void MyHandler_ProperlyReplacesRenderer()
{
    // Assert
    HandlerMigrationAssert.RendererReplacedByHandler(
        "MyRenderer",
        typeof(MyHandler),
        typeof(MyView));
    
    HandlerMigrationAssert.OnElementChangedMigrated(typeof(MyHandler));
    HandlerMigrationAssert.OnElementPropertyChangedMigrated(typeof(MyHandler));
}
```

### Platform-Specific Test
```csharp
[Fact]
public void MyHandler_Android_CreatesCorrectNativeView()
{
    // Arrange
    PlatformTestHelpers.RequirePlatform("Android");
    var handler = new MyHandler();
    
    // Assert
    HandlerMigrationAssert.NativeViewIsCorrectType<Android.Views.View>(handler);
    Assert.True(PlatformTestHelpers.IsValidHandlerForPlatform(typeof(MyHandler)));
}
```

### Bindable Property Migration
```csharp
[Fact]
public void MyControl_TextProperty_IsMigrated()
{
    // Arrange & Assert
    MauiAssert.HasBindableProperty(
        typeof(MyControl),
        "Text",
        typeof(string),
        string.Empty);
    
    MauiAssert.DependencyPropertyMigrated(
        typeof(XamarinMyControl),
        typeof(MyControl),
        "Text");
    
    // Verify data binding works
    var control = new MyControl();
    MauiAssert.SupportsDataBinding(control, MyControl.TextProperty);
}
```

### Complete Handler Test Example
```csharp
[Fact]
public void MyHandler_CompleteValidation()
{
    // 1. Validate handler replaces renderer
    HandlerMigrationAssert.RendererReplacedByHandler(
        "MyRenderer",
        typeof(MyHandler),
        typeof(MyView));
    
    // 2. Validate migration patterns
    HandlerMigrationAssert.OnElementChangedMigrated(typeof(MyHandler));
    HandlerMigrationAssert.OnElementPropertyChangedMigrated(typeof(MyHandler));
    HandlerMigrationAssert.DisposePatternsPreserved(typeof(MyHandler));
    
    // 3. Validate platform implementations
    HandlerMigrationAssert.HasAllPlatformImplementations(
        typeof(MyHandler),
        "Android", "iOS", "Windows");
    
    // 4. Validate handler structure
    var handler = new MyHandler();
    HandlerMigrationAssert.HandlerDisconnectsProperly(handler);
}
```

## Test Context Notes

### Unit Test Limitations
These helpers are designed for **unit tests** that run without a full MAUI application context:
- Handler.PlatformView will be null in unit tests
- IMauiContext is unavailable in unit tests
- Platform-specific code cannot fully execute in unit tests

### Integration Test Scenarios
For tests requiring full MAUI app context:
- Use the DI integration tests pattern (see `DependencyInjectionTests.cs`)
- Some assertions will throw `Xunit.SkipException` when app context is unavailable
- Platform-specific tests should use `PlatformTestHelpers.RequirePlatform()`

### Assertion Behavior
- Most assertions validate **type structure** and **interface compliance** in unit test context
- Full functional validation requires integration tests with MAUI app host
- Assertions focus on migration **correctness** (patterns, conventions, types) not runtime behavior

## Best Practices

1. **Use MauiAssert for control-level validation**
   - Control initialization
   - Bindable properties
   - Layout properties
   - Namespace migrations

2. **Use HandlerMigrationAssert for renderer→handler migrations**
   - Handler structure
   - Mapper configuration
   - Platform implementations
   - Migration pattern compliance

3. **Use PlatformTestHelpers for platform-specific scenarios**
   - Platform detection
   - Conditional test execution
   - Native type validation
   - Test element creation

4. **Combine helpers for comprehensive testing**
   ```csharp
   // Validate control
   MauiAssert.IsValidMauiControl(control);
   
   // Validate handler
   HandlerMigrationAssert.RendererReplacedByHandler(...);
   
   // Validate platform
   PlatformTestHelpers.RequirePlatform("Android");
   ```

## See Also
- `HandlerInfrastructureTests.cs` - Example of handler testing patterns (9/9 passing)
- `DependencyInjectionTests.cs` - Example of DI integration testing (8/11 passing)
- Phase 2 Foundation documentation in `docs/tasks.md`
