using Brupper.Maui.Handlers;
using Microsoft.Maui;
using Xunit;

namespace Brupper.Maui.Tests.Infrastructure;

/// <summary>
/// Tests for base handler infrastructure
/// </summary>
public class HandlerInfrastructureTests
{
    [Fact]
    public void PlatformHandlerBase_InitialState_IsNotConnected()
    {
        // Arrange
        var handler = new TestHandler();

        // Assert
        Assert.Null(handler.PlatformView);
        Assert.Null(handler.VirtualView);
        Assert.False(handler.TestIsConnected);
    }

    [Fact]
    public void PlatformHandlerBase_ConnectHandler_CreatesPlatformView()
    {
        // Arrange
        var handler = new TestHandler();
        var virtualView = new TestView();

        // Act
        handler.ConnectHandler(virtualView);

        // Assert
        Assert.NotNull(handler.PlatformView);
        Assert.Same(virtualView, handler.VirtualView);
        Assert.True(handler.TestIsConnected);
    }

    [Fact]
    public void PlatformHandlerBase_ConnectHandler_ThrowsOnNullVirtualView()
    {
        // Arrange
        var handler = new TestHandler();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => handler.ConnectHandler(null!));
    }

    [Fact]
    public void PlatformHandlerBase_DisconnectHandler_CleansUpResources()
    {
        // Arrange
        var handler = new TestHandler();
        var virtualView = new TestView();
        handler.ConnectHandler(virtualView);

        // Act
        handler.DisconnectHandler();

        // Assert
        Assert.Null(handler.PlatformView);
        Assert.Null(handler.VirtualView);
        Assert.False(handler.TestIsConnected);
    }

    [Fact]
    public void PlatformHandlerBase_DisconnectHandler_SafeWhenNotConnected()
    {
        // Arrange
        var handler = new TestHandler();

        // Act & Assert (should not throw)
        handler.DisconnectHandler();
    }

    [Fact]
    public void PlatformHandlerBase_ConnectHandler_DisconnectsExistingConnection()
    {
        // Arrange
        var handler = new TestHandler();
        var virtualView1 = new TestView();
        var virtualView2 = new TestView();
        
        handler.ConnectHandler(virtualView1);
        var firstPlatformView = handler.PlatformView;

        // Act
        handler.ConnectHandler(virtualView2);

        // Assert
        Assert.Same(virtualView2, handler.VirtualView);
        Assert.NotSame(firstPlatformView, handler.PlatformView);
        Assert.True(handler.DisconnectedCalled);
    }

    [Fact]
    public void PlatformHandlerBase_Lifecycle_CallsHooks()
    {
        // Arrange
        var handler = new TestHandler();
        var virtualView = new TestView();

        // Act - Connect
        handler.ConnectHandler(virtualView);

        // Assert - OnConnected called
        Assert.True(handler.ConnectedCalled);
        Assert.False(handler.DisconnectingCalled);
        Assert.False(handler.DisconnectedCalled);

        // Reset
        handler.ConnectedCalled = false;

        // Act - Disconnect
        handler.DisconnectHandler();

        // Assert - Disconnect hooks called
        Assert.False(handler.ConnectedCalled);
        Assert.True(handler.DisconnectingCalled);
        Assert.True(handler.DisconnectedCalled);
    }

    [Fact]
    public void PlatformHandlerBase_UpdateProperty_RequiresConnection()
    {
        // Arrange
        var handler = new TestHandler();

        // Act
        handler.TestUpdateProperty("TestProperty");

        // Assert - Should not call OnPropertyChanged when not connected
        Assert.False(handler.PropertyChangedCalled);
    }

    [Fact]
    public void PlatformHandlerBase_UpdateProperty_CallsOnPropertyChanged()
    {
        // Arrange
        var handler = new TestHandler();
        var virtualView = new TestView();
        handler.ConnectHandler(virtualView);

        // Act
        handler.TestUpdateProperty("TestProperty");

        // Assert
        Assert.True(handler.PropertyChangedCalled);
        Assert.Equal("TestProperty", handler.LastPropertyChanged);
    }

    // Test Implementations
    private class TestHandler : PlatformHandlerBase<object, IView>
    {
        public bool ConnectedCalled { get; set; }
        public bool DisconnectingCalled { get; set; }
        public bool DisconnectedCalled { get; set; }
        public bool PropertyChangedCalled { get; set; }
        public string? LastPropertyChanged { get; set; }

        public bool TestIsConnected => IsConnected;

        public void TestUpdateProperty(string propertyName) => UpdateProperty(propertyName);

        public override object CreatePlatformView() => new object();

        protected override void OnConnected()
        {
            base.OnConnected();
            ConnectedCalled = true;
        }

        protected override void OnDisconnecting()
        {
            base.OnDisconnecting();
            DisconnectingCalled = true;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            DisconnectedCalled = true;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            PropertyChangedCalled = true;
            LastPropertyChanged = propertyName;
        }
    }

    private class TestView : IView
    {
        public IElement? Parent { get; set; }
        public IViewHandler? Handler { get; set; }
        IElementHandler? IElement.Handler { get => Handler; set => Handler = value as IViewHandler; }
        
        // IView required properties (MAUI 9.0)
        public string AutomationId { get; set; } = string.Empty;
        public FlowDirection FlowDirection { get; set; }
        public Microsoft.Maui.Primitives.LayoutAlignment HorizontalLayoutAlignment { get; set; }
        public Microsoft.Maui.Primitives.LayoutAlignment VerticalLayoutAlignment { get; set; }
        public Semantics? Semantics { get; set; }
        
        // Size and Layout properties
        public double Width { get; set; }
        public double Height { get; set; }
        public Thickness Margin { get; set; }
        public double MaximumHeight { get; set; } = double.PositiveInfinity;
        public double MaximumWidth { get; set; } = double.PositiveInfinity;
        public double MinimumHeight { get; set; }
        public double MinimumWidth { get; set; }
        public Size DesiredSize { get; set; }
        public Rect Frame { get; set; }
        
        // Visual properties
        public double Opacity { get; set; } = 1.0;
        public Paint? Background { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public IShape? Clip { get; set; }
        public IShadow? Shadow { get; set; }
        public int ZIndex { get; set; }
        
        // Transform properties (ITransform)
        public double TranslationX { get; set; }
        public double TranslationY { get; set; }
        public double Scale { get; set; } = 1.0;
        public double ScaleX { get; set; } = 1.0;
        public double ScaleY { get; set; } = 1.0;
        public double Rotation { get; set; }
        public double RotationX { get; set; }
        public double RotationY { get; set; }
        public double AnchorX { get; set; } = 0.5;
        public double AnchorY { get; set; } = 0.5;
        
        // Focus and input
        public bool IsFocused { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool InputTransparent { get; set; }

        // Layout methods
        public Size Arrange(Rect bounds)
        {
            Frame = bounds;
            return bounds.Size;
        }
        
        public Size Measure(double widthConstraint, double heightConstraint)
        {
            DesiredSize = new Size(widthConstraint, heightConstraint);
            return DesiredSize;
        }
        
        public void InvalidateMeasure() { }
        public void InvalidateArrange() { }
        
        // Focus methods
        public bool Focus()
        {
            IsFocused = true;
            return true;
        }
        
        public void Unfocus()
        {
            IsFocused = false;
        }
    }
}
