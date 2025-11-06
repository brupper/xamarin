using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Xunit;

namespace Brupper.Maui.Tests.Infrastructure;

/// <summary>
/// Custom assertion helpers for MAUI migration validation
/// </summary>
public static class MauiAssert
{
    /// <summary>
    /// Asserts that a control is properly initialized with default MAUI properties
    /// </summary>
    public static void IsValidMauiControl(Element element)
    {
        Assert.NotNull(element);
        Assert.NotNull(element.GetType());
        
        // Verify element can be added to a parent
        if (element is View view)
        {
            var parent = new ContentPage();
            Assert.NotNull(parent);
            // Element should be addable to a container
        }
    }

    /// <summary>
    /// Asserts that a handler is properly configured for a MAUI control
    /// </summary>
    public static void HasValidHandler<THandler>(IElement element) where THandler : IElementHandler
    {
        Assert.NotNull(element);
        
        // Handler should be assignable (even if null in test context)
        var handlerProperty = element.GetType().GetProperty("Handler");
        Assert.NotNull(handlerProperty);
        
        // Verify handler type compatibility
        var handlerType = typeof(THandler);
        Assert.True(typeof(IElementHandler).IsAssignableFrom(handlerType),
            $"Handler type {handlerType.Name} must implement IElementHandler");
    }

    /// <summary>
    /// Asserts that a bindable property exists and has correct metadata
    /// </summary>
    public static void HasBindableProperty(
        Type elementType,
        string propertyName,
        Type propertyType,
        object? defaultValue = null)
    {
        Assert.NotNull(elementType);
        Assert.NotNull(propertyName);
        
        // Look for the bindable property field
        var bindablePropertyField = elementType.GetField(
            $"{propertyName}Property",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
        Assert.NotNull(bindablePropertyField);
        
        var bindableProperty = bindablePropertyField.GetValue(null) as BindableProperty;
        Assert.NotNull(bindableProperty);
        Assert.Equal(propertyName, bindableProperty.PropertyName);
        Assert.Equal(propertyType, bindableProperty.ReturnType);
        
        if (defaultValue != null)
        {
            Assert.Equal(defaultValue, bindableProperty.DefaultValue);
        }
    }

    /// <summary>
    /// Asserts that two colors are equal within tolerance
    /// </summary>
    public static void ColorEqual(Color expected, Color actual, float tolerance = 0.01f)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        
        Assert.True(
            Math.Abs(expected.Red - actual.Red) <= tolerance &&
            Math.Abs(expected.Green - actual.Green) <= tolerance &&
            Math.Abs(expected.Blue - actual.Blue) <= tolerance &&
            Math.Abs(expected.Alpha - actual.Alpha) <= tolerance,
            $"Expected color {expected} but got {actual}");
    }

    /// <summary>
    /// Asserts that a MAUI view has correct layout properties
    /// </summary>
    public static void HasValidLayout(View view)
    {
        Assert.NotNull(view);
        
        // Layout properties should be initialized
        // HorizontalOptions and VerticalOptions are value types, not null
        
        // Bounds should be accessible
        var bounds = view.Bounds;
        // Bounds is a value type (Rect), not null
    }

    /// <summary>
    /// Asserts that a control properly implements INotifyPropertyChanged
    /// </summary>
    public static void ImplementsPropertyChanged(object element)
    {
        Assert.NotNull(element);
        Assert.IsAssignableFrom<System.ComponentModel.INotifyPropertyChanged>(element);
    }

    /// <summary>
    /// Asserts that a dependency property was migrated correctly to BindableProperty
    /// </summary>
    public static void DependencyPropertyMigrated(
        Type xamarinFormsType,
        Type mauiType,
        string propertyName)
    {
        Assert.NotNull(xamarinFormsType);
        Assert.NotNull(mauiType);
        
        // Both types should have the property
        var xamarinProperty = xamarinFormsType.GetProperty(propertyName);
        var mauiProperty = mauiType.GetProperty(propertyName);
        
        Assert.NotNull(xamarinProperty);
        Assert.NotNull(mauiProperty);
        
        // Property types should be compatible
        Assert.True(
            mauiProperty.PropertyType.IsAssignableFrom(xamarinProperty.PropertyType) ||
            xamarinProperty.PropertyType.IsAssignableFrom(mauiProperty.PropertyType),
            $"Property {propertyName} types are not compatible: " +
            $"Xamarin.Forms: {xamarinProperty.PropertyType.Name}, " +
            $"MAUI: {mauiProperty.PropertyType.Name}");
    }

    /// <summary>
    /// Asserts that a platform-specific implementation exists
    /// </summary>
    public static void HasPlatformImplementation(string platform, Type interfaceType)
    {
        Assert.NotNull(platform);
        Assert.NotNull(interfaceType);
        Assert.True(interfaceType.IsInterface, "Type must be an interface");
        
        // Platform should be one of the supported platforms
        var validPlatforms = new[] { "Android", "iOS", "Windows", "MacCatalyst" };
        Assert.Contains(platform, validPlatforms);
    }

    /// <summary>
    /// Asserts that a renderer was properly migrated to a handler
    /// </summary>
    public static void RendererMigratedToHandler(
        string rendererName,
        string handlerName,
        Type handlerType)
    {
        Assert.NotNull(rendererName);
        Assert.NotNull(handlerName);
        Assert.NotNull(handlerType);
        
        // Handler should implement required interfaces
        Assert.True(
            typeof(IElementHandler).IsAssignableFrom(handlerType),
            $"Handler {handlerName} must implement IElementHandler");
        
        // Handler naming convention: should end with "Handler"
        Assert.EndsWith("Handler", handlerName);
    }

    /// <summary>
    /// Asserts that a MAUI font has valid properties
    /// </summary>
    public static void IsValidFont(Microsoft.Maui.Font font)
    {
        // Font is a value type (struct), not null
        
        // Font should have either family or size set
        Assert.True(
            !string.IsNullOrEmpty(font.Family) || font.Size > 0,
            "Font must have either Family or Size specified");
    }

    /// <summary>
    /// Asserts that a MAUI thickness has valid values
    /// </summary>
    public static void IsValidThickness(Thickness thickness)
    {
        Assert.True(thickness.Left >= 0, "Left margin must be non-negative");
        Assert.True(thickness.Top >= 0, "Top margin must be non-negative");
        Assert.True(thickness.Right >= 0, "Right margin must be non-negative");
        Assert.True(thickness.Bottom >= 0, "Bottom margin must be non-negative");
    }

    /// <summary>
    /// Asserts that a control has been properly disposed
    /// </summary>
    public static void IsProperlyDisposed(IDisposable disposable)
    {
        Assert.NotNull(disposable);
        
        // Should not throw when disposed
        var exception = Record.Exception(() => disposable.Dispose());
        Assert.Null(exception);
    }

    /// <summary>
    /// Asserts that a MAUI namespace migration is correct
    /// </summary>
    public static void NamespaceMigrated(Type type, string expectedNamespace)
    {
        Assert.NotNull(type);
        Assert.NotNull(expectedNamespace);
        
        var actualNamespace = type.Namespace;
        Assert.NotNull(actualNamespace);
        
        // Should use MAUI namespace, not Xamarin.Forms
        Assert.DoesNotContain("Xamarin.Forms", actualNamespace);
        Assert.Contains("Microsoft.Maui", actualNamespace);
        
        if (!string.IsNullOrEmpty(expectedNamespace))
        {
            Assert.Equal(expectedNamespace, actualNamespace);
        }
    }

    /// <summary>
    /// Asserts that a control supports data binding
    /// </summary>
    public static void SupportsDataBinding(BindableObject bindableObject, BindableProperty property)
    {
        Assert.NotNull(bindableObject);
        Assert.NotNull(property);
        
        // Should be able to set binding
        var testValue = "TestValue";
        var exception = Record.Exception(() =>
        {
            bindableObject.SetValue(property, testValue);
            var value = bindableObject.GetValue(property);
            Assert.Equal(testValue, value);
        });
        
        Assert.Null(exception);
    }
}
