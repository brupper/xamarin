using Brupper.Diagnostics;
using Brupper.Maui.Diagnostics;
using Brupper.Maui.Services;
using Brupper.Maui.Services.Implementations;
using Brupper.Maui.ViewModels;
using Brupper.Maui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Brupper.Maui.Tests.US1;

/// <summary>
/// Tests for service registration and dependency injection configuration.
/// Validates that all core services can be resolved from the DI container.
/// </summary>
public class ServiceRegistrationTests
{
    /// <summary>
    /// Helper method to create a service collection with Brupper MAUI services registered.
    /// </summary>
    private static ServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        return services;
    }

    [Fact]
    public void AddBrupperMaui_Should_Register_Core_Services()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Core services should be resolvable
        Assert.NotNull(provider.GetService<INavigationService>());
        Assert.NotNull(provider.GetService<IConnectivity>());
        Assert.NotNull(provider.GetService<IFileSystem>());
        Assert.NotNull(provider.GetService<ILocalizationService>());
    }

    [Fact]
    public void AddBrupperMaui_Should_Register_Platform_Services()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Platform-specific services should be resolvable
        Assert.NotNull(provider.GetService<IPlatformInformationService>());
        Assert.NotNull(provider.GetService<IPermissionHelper>());
        Assert.NotNull(provider.GetService<IImageResizer>());
        Assert.NotNull(provider.GetService<IOutputRendererServices>());
    }

    [Fact]
    public void AddBrupperMaui_Should_Register_Diagnostics_Services()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.NotNull(provider.GetService<IDiagnosticsPlatformInformationProvider>());
        Assert.NotNull(provider.GetService<IDiagnosticsStorage>());
        Assert.NotNull(provider.GetService<FormsLogger>());
        Assert.NotNull(provider.GetService<ILogger>());
    }

    [Fact]
    public void AddBrupperMaui_Should_Register_Microsoft_Logging()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var loggerFactory = provider.GetService<ILoggerFactory>();
        var logger = provider.GetService<ILogger<ServiceRegistrationTests>>();

        // Assert
        Assert.NotNull(loggerFactory);
        Assert.NotNull(logger);
    }

    [Fact]
    public void NavigationService_Should_Be_Singleton()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var service1 = provider.GetService<INavigationService>();
        var service2 = provider.GetService<INavigationService>();

        // Assert - Same instance should be returned
        Assert.Same(service1, service2);
    }

    [Fact]
    public void PlatformInformationService_Should_Be_Singleton()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var service1 = provider.GetService<IPlatformInformationService>();
        var service2 = provider.GetService<IPlatformInformationService>();

        // Assert - Same instance should be returned
        Assert.Same(service1, service2);
    }

    [Fact]
    public void LocalizationService_Should_Be_Resolvable()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var localizationService = provider.GetService<ILocalizationService>();

        // Assert
        Assert.NotNull(localizationService);
        Assert.IsType<LocalizationService>(localizationService);
    }

    [Fact]
    public void LocalizationService_Should_Return_Key_When_Resource_Not_Found()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();
        var localizationService = provider.GetRequiredService<ILocalizationService>();

        // Act
        var result = localizationService.GetText("NonExistentKey");

        // Assert
        Assert.Equal("NonExistentKey", result);
    }

    [Fact]
    public void LocalizationService_Should_Format_Text_With_Args()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();
        var localizationService = provider.GetRequiredService<ILocalizationService>();

        // Act - Since resource won't exist, it will return the key with formatting applied
        var result = localizationService.GetText("Hello {0}", "World");

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void AddViewModel_Should_Register_ViewModel_As_Transient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        services.AddViewModel<TestViewModel>();
        var provider = services.BuildServiceProvider();

        // Act
        var vm1 = provider.GetService<TestViewModel>();
        var vm2 = provider.GetService<TestViewModel>();

        // Assert - Different instances should be returned
        Assert.NotNull(vm1);
        Assert.NotNull(vm2);
        Assert.NotSame(vm1, vm2);
    }

    [Fact]
    public void AddViewModelSingleton_Should_Register_ViewModel_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        services.AddViewModelSingleton<TestViewModel>();
        var provider = services.BuildServiceProvider();

        // Act
        var vm1 = provider.GetService<TestViewModel>();
        var vm2 = provider.GetService<TestViewModel>();

        // Assert - Same instance should be returned
        Assert.NotNull(vm1);
        Assert.NotNull(vm2);
        Assert.Same(vm1, vm2);
    }

    [Fact]
    public void AddView_Should_Register_View_As_Transient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        services.AddView<TestPage>();
        var provider = services.BuildServiceProvider();

        // Act
        var page1 = provider.GetService<TestPage>();
        var page2 = provider.GetService<TestPage>();

        // Assert - Different instances should be returned
        Assert.NotNull(page1);
        Assert.NotNull(page2);
        Assert.NotSame(page1, page2);
    }

    [Fact]
    public void AddViewWithViewModel_Should_Register_Both_As_Transient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBrupperMaui();
        services.AddViewWithViewModel<TestPage, TestViewModel>();
        var provider = services.BuildServiceProvider();

        // Act
        var page = provider.GetService<TestPage>();
        var viewModel = provider.GetService<TestViewModel>();

        // Assert
        Assert.NotNull(page);
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void DiagnosticsPlatformInformationProvider_Should_Resolve_To_PlatformInformationService()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var diagnosticsProvider = provider.GetService<IDiagnosticsPlatformInformationProvider>();
        var platformService = provider.GetService<IPlatformInformationService>();

        // Assert - Should be the same instance
        Assert.NotNull(diagnosticsProvider);
        Assert.NotNull(platformService);
        Assert.Same(diagnosticsProvider, platformService);
    }

    [Fact]
    public void BrupperLogger_Should_Be_Initialized()
    {
        // Arrange
        var services = CreateServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var logger = provider.GetService<ILogger>();

        // Assert
        Assert.NotNull(logger);
        Assert.Same(Logger.Current, logger);
    }

    // Test helper classes
    private class TestViewModel : ViewModelBase
    {
        public TestViewModel(ILogger logger, INavigationService navigationService)
            : base(logger, navigationService)
        {
        }
    }

    private class TestPage : BasePage<TestViewModel>
    {
    }
}
