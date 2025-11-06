using Brupper.Diagnostics;
using Brupper.Maui.Diagnostics;
using Brupper.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Brupper.Maui.Tests;

/// <summary>
/// Tests for Microsoft.Extensions.DependencyInjection integration
/// </summary>
public class ServiceRegistrationTests
{
    [Fact]
    public void AddBrupperMaui_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrupperMaui();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify public diagnostic services are registered
        Assert.NotNull(serviceProvider.GetService<IDiagnosticsPlatformInformationProvider>());
        Assert.NotNull(serviceProvider.GetService<IDiagnosticsStorage>());
        Assert.NotNull(serviceProvider.GetService<ILogger>());
        
        // Verify concrete logger implementation (FormsLogger is internal)
        var logger = serviceProvider.GetService<ILogger>();
        Assert.Equal("Brupper.Maui.Diagnostics.FormsLogger", logger?.GetType().FullName);
    }

    [Fact]
    public void AddBrupperMaui_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddBrupperMaui();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddBrupperMaui_WithOptions_ConfiguresServices()
    {
        // Arrange
        var services = new ServiceCollection();
        bool optionsCalled = false;

        // Act
        services.AddBrupperMaui<TestOptions>(options =>
        {
            optionsCalled = true;
            options.Value = "Test";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.True(optionsCalled);
        Assert.NotNull(serviceProvider.GetService<ILogger>());
    }

    [Fact]
    public void AddBrupperMaui_ThrowsOnNullServices()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddBrupperMaui());
    }

    [Fact]
    public void AddBrupperMaui_WithOptions_ThrowsOnNullServices()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            services.AddBrupperMaui<TestOptions>(_ => { }));
    }

    [Fact]
    public void AddBrupperMaui_WithOptions_ThrowsOnNullAction()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            services.AddBrupperMaui<TestOptions>(null!));
    }

    [Fact]
    public void ServiceRegistration_AllowsMethodChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services
            .AddBrupperMaui()
            .AddSingleton<ITestService, TestService>();

        // Assert
        Assert.NotNull(result);
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<ITestService>());
    }

    [Fact]
    public void ServiceRegistration_RegistersSingletonServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var logger1 = serviceProvider.GetService<ILogger>();
        var logger2 = serviceProvider.GetService<ILogger>();

        // Assert
        Assert.Same(logger1, logger2);
    }

    private class TestOptions
    {
        public string? Value { get; set; }
    }

    private interface ITestService { }
    private class TestService : ITestService { }
}
