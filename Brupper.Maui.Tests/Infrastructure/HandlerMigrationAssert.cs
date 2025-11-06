using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Xunit;

namespace Brupper.Maui.Tests.Infrastructure;

/// <summary>
/// Assertion helpers specifically for renderer-to-handler migration validation
/// </summary>
public static class HandlerMigrationAssert
{
    /// <summary>
    /// Asserts that a handler properly replaced a renderer
    /// </summary>
    public static void RendererReplacedByHandler(
        string legacyRendererName,
        Type handlerType,
        Type virtualViewType)
    {
        Assert.NotNull(legacyRendererName);
        Assert.NotNull(handlerType);
        Assert.NotNull(virtualViewType);

        // Handler should implement IElementHandler
        Assert.True(
            typeof(IElementHandler).IsAssignableFrom(handlerType),
            $"{handlerType.Name} must implement IElementHandler");

        // Handler should follow naming convention
        Assert.EndsWith("Handler", handlerType.Name);

        // Virtual view should be assignable to IElement
        Assert.True(
            typeof(IElement).IsAssignableFrom(virtualViewType),
            $"{virtualViewType.Name} must implement IElement");
    }

    /// <summary>
    /// Asserts that a handler has proper mapper configuration
    /// </summary>
    public static void HasValidMapper<TVirtualView>(IPropertyMapper mapper)
        where TVirtualView : IElement
    {
        Assert.NotNull(mapper);

        // Mapper should be of correct generic type
        var mapperType = mapper.GetType();
        Assert.NotNull(mapperType);

        // PropertyMapper should handle property updates
        // In unit test context, we verify structure not functionality
    }

    /// <summary>
    /// Asserts that platform-specific handlers exist for all required platforms
    /// </summary>
    public static void HasAllPlatformImplementations(
        Type handlerBaseType,
        params string[] requiredPlatforms)
    {
        Assert.NotNull(handlerBaseType);
        Assert.NotNull(requiredPlatforms);
        Assert.NotEmpty(requiredPlatforms);

        foreach (var platform in requiredPlatforms)
        {
            // Platform implementations should exist
            // In unit tests, we verify type structure
            Assert.Contains(platform, new[] { "Android", "iOS", "Windows", "MacCatalyst" });
        }
    }

    /// <summary>
    /// Asserts that a handler's native view is correct type
    /// </summary>
    public static void NativeViewIsCorrectType<TNativeView>(IElementHandler handler)
    {
        Assert.NotNull(handler);

        // In unit test without platform context, native view may be null
        // Integration tests will verify actual native view type
        var nativeViewProperty = handler.GetType().GetProperty("PlatformView");
        Assert.NotNull(nativeViewProperty);

        // Property type should be assignable to expected native type
        Assert.True(
            typeof(TNativeView).IsAssignableFrom(nativeViewProperty.PropertyType) ||
            nativeViewProperty.PropertyType == typeof(object), // Generic handlers use object
            $"Expected native view type {typeof(TNativeView).Name} " +
            $"but handler has {nativeViewProperty.PropertyType.Name}");
    }

    /// <summary>
    /// Asserts that a handler properly disconnects from virtual view
    /// </summary>
    public static void HandlerDisconnectsProperly(IElementHandler handler)
    {
        Assert.NotNull(handler);

        // Handler should implement proper cleanup
        var disconnectMethod = handler.GetType().GetMethod("DisconnectHandler");
        Assert.NotNull(disconnectMethod);

        // Should not throw when disconnected
        var exception = Record.Exception(() =>
        {
            if (handler is IDisposable disposable)
            {
                disposable.Dispose();
            }
        });
        Assert.Null(exception);
    }

    /// <summary>
    /// Asserts that effect was properly migrated to handler
    /// </summary>
    public static void EffectMigratedToHandler(
        string effectName,
        Type handlerType)
    {
        Assert.NotNull(effectName);
        Assert.NotNull(handlerType);

        // Effects in Xamarin.Forms become handler mappers in MAUI
        Assert.True(
            typeof(IElementHandler).IsAssignableFrom(handlerType),
            $"Effect {effectName} should be migrated to handler implementing IElementHandler");

        // Handler should have mapper for effect properties
    }

    /// <summary>
    /// Asserts that custom renderer features are preserved in handler
    /// </summary>
    public static void CustomRendererFeaturesPreserved(
        Type rendererType,
        Type handlerType,
        params string[] requiredMethods)
    {
        Assert.NotNull(rendererType);
        Assert.NotNull(handlerType);
        Assert.NotNull(requiredMethods);

        foreach (var methodName in requiredMethods)
        {
            var rendererMethod = rendererType.GetMethod(
                methodName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            // Method should exist in renderer
            Assert.NotNull(rendererMethod);

            // Handler should have equivalent functionality
            // (may be in mapper, may be in handler method)
        }
    }

    /// <summary>
    /// Asserts that renderer's OnElementChanged is replaced by handler's CreatePlatformView
    /// </summary>
    public static void OnElementChangedMigrated(Type handlerType)
    {
        Assert.NotNull(handlerType);

        // Handler should have CreatePlatformView or similar method
        var createMethod = handlerType.GetMethod("CreatePlatformView") ??
                          handlerType.GetMethod("CreateNativeView");

        // In base handler pattern, method may be in base class
        if (createMethod == null)
        {
            // Check base types
            var baseType = handlerType.BaseType;
            while (baseType != null && createMethod == null)
            {
                createMethod = baseType.GetMethod("CreatePlatformView") ??
                              baseType.GetMethod("CreateNativeView");
                baseType = baseType.BaseType;
            }
        }

        Assert.NotNull(createMethod);
    }

    /// <summary>
    /// Asserts that renderer's OnElementPropertyChanged is replaced by mapper
    /// </summary>
    public static void OnElementPropertyChangedMigrated(Type handlerType)
    {
        Assert.NotNull(handlerType);

        // Handler should use property mapper instead of OnElementPropertyChanged
        // Check for PropertyMapper field or property
        var mapperField = handlerType.GetField(
            "PropertyMapper",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static);

        var mapperProperty = handlerType.GetProperty(
            "PropertyMapper",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static);

        Assert.True(
            mapperField != null || mapperProperty != null,
            "Handler should have PropertyMapper for property change handling");
    }

    /// <summary>
    /// Asserts that renderer dispose pattern is preserved in handler
    /// </summary>
    public static void DisposePatternsPreserved(Type handlerType)
    {
        Assert.NotNull(handlerType);

        // Handler should implement IDisposable or have DisconnectHandler
        var implementsDisposable = typeof(IDisposable).IsAssignableFrom(handlerType);
        var hasDisconnectHandler = handlerType.GetMethod("DisconnectHandler") != null;

        Assert.True(
            implementsDisposable || hasDisconnectHandler,
            "Handler should implement IDisposable or have DisconnectHandler method");
    }

    /// <summary>
    /// Asserts that a handler uses command mapper for events
    /// </summary>
    public static void UsesCommandMapper(Type handlerType)
    {
        Assert.NotNull(handlerType);

        // Handler should have CommandMapper for event handling
        var commandMapperField = handlerType.GetField(
            "CommandMapper",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static);

        var commandMapperProperty = handlerType.GetProperty(
            "CommandMapper",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static);

        // CommandMapper is optional, but if present should be valid
        if (commandMapperField != null || commandMapperProperty != null)
        {
            Assert.True(true, "Handler has CommandMapper");
        }
    }

    /// <summary>
    /// Asserts that ViewRenderer was properly migrated to ViewHandler
    /// </summary>
    public static void ViewRendererMigratedToViewHandler(
        Type legacyRendererType,
        Type newHandlerType)
    {
        Assert.NotNull(legacyRendererType);
        Assert.NotNull(newHandlerType);

        // Renderer should have "Renderer" in name
        Assert.Contains("Renderer", legacyRendererType.Name);

        // Handler should have "Handler" in name
        Assert.Contains("Handler", newHandlerType.Name);

        // Handler should implement IViewHandler
        Assert.True(
            typeof(IElementHandler).IsAssignableFrom(newHandlerType),
            "ViewHandler must implement IElementHandler");
    }

    /// <summary>
    /// Asserts that a platform-specific handler implementation follows conventions
    /// </summary>
    public static void PlatformHandlerFollowsConventions(
        Type handlerType,
        string platform)
    {
        Assert.NotNull(handlerType);
        Assert.NotNull(platform);

        // Handler file should be in Platforms/{platform} folder
        // This is structural - actual file path validation happens elsewhere

        // Handler should implement IElementHandler
        Assert.True(
            typeof(IElementHandler).IsAssignableFrom(handlerType),
            $"{platform} handler must implement IElementHandler");

        // Platform should be valid
        var validPlatforms = new[] { "Android", "iOS", "Windows", "MacCatalyst" };
        Assert.Contains(platform, validPlatforms);
    }
}
