using Brupper.Maui.Services;
using Brupper.Maui.ViewModels;
using Brupper.Maui.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using Moq;

namespace Brupper.Maui.Tests.US1;

/// <summary>
/// Tests for lifecycle event integration between Pages/Popups and ViewModels.
/// Verifies that MAUI lifecycle events properly invoke ViewModel lifecycle methods.
/// Part of Phase 3 User Story 1: Core App Functionality Preservation.
/// </summary>
public class LifecycleIntegrationTests
{
    #region BasePage Lifecycle Tests

    [Fact]
    public void BasePage_OnAppearing_Should_Invoke_ViewModel_ViewAppearing_And_ViewAppeared()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);
        
        var page = new TestPage
        {
            ViewModel = viewModel
        };

        // Act
        page.TriggerAppearing();

        // Assert
        Assert.True(viewModel.ViewAppearingCalled, "ViewAppearing should have been called");
        Assert.True(viewModel.ViewAppearedCalled, "ViewAppeared should have been called");
    }

    [Fact]
    public void BasePage_OnDisappearing_Should_Invoke_ViewModel_ViewDisappearing_And_ViewDisappeared()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);
        
        var page = new TestPage
        {
            ViewModel = viewModel
        };

        // Act
        page.TriggerDisappearing();

        // Assert
        Assert.True(viewModel.ViewDisappearingCalled, "ViewDisappearing should have been called");
        Assert.True(viewModel.ViewDisappearedCalled, "ViewDisappeared should have been called");
    }

    [Fact]
    public void BasePage_Lifecycle_Should_Call_Methods_In_Correct_Order()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);
        
        var page = new TestPage
        {
            ViewModel = viewModel
        };

        // Act
        page.TriggerAppearing();
        page.TriggerDisappearing();

        // Assert
        Assert.Equal(4, viewModel.LifecycleCallOrder.Count);
        Assert.Equal("ViewAppearing", viewModel.LifecycleCallOrder[0]);
        Assert.Equal("ViewAppeared", viewModel.LifecycleCallOrder[1]);
        Assert.Equal("ViewDisappearing", viewModel.LifecycleCallOrder[2]);
        Assert.Equal("ViewDisappeared", viewModel.LifecycleCallOrder[3]);
    }

    [Fact]
    public void BasePage_Should_Not_Throw_If_ViewModel_Is_Null()
    {
        // Arrange
        var page = new TestPage
        {
            ViewModel = null
        };

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
        {
            page.TriggerAppearing();
            page.TriggerDisappearing();
        });

        Assert.Null(exception);
    }

    #endregion

    #region BasePopup Lifecycle Tests

    [Fact]
    public void BasePopup_OnOpened_Should_Invoke_ViewModel_ViewAppearing_And_ViewAppeared()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);
        
        var popup = new TestPopup
        {
            ViewModel = viewModel
        };

        // Act
        popup.TriggerOpened();

        // Assert
        Assert.True(viewModel.ViewAppearingCalled, "ViewAppearing should have been called on popup opened");
        Assert.True(viewModel.ViewAppearedCalled, "ViewAppeared should have been called on popup opened");
    }

    [Fact]
    public void BasePopup_OnClosed_Should_Invoke_ViewModel_ViewDisappearing_And_ViewDisappeared()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);
        
        var popup = new TestPopup
        {
            ViewModel = viewModel
        };

        // Act
        popup.TriggerClosed(wasDismissedByTappingOutside: false);

        // Assert
        Assert.True(viewModel.ViewDisappearingCalled, "ViewDisappearing should have been called on popup closed");
        Assert.True(viewModel.ViewDisappearedCalled, "ViewDisappeared should have been called on popup closed");
    }

    [Fact]
    public void BasePopup_Should_Not_Throw_If_ViewModel_Is_Null()
    {
        // Arrange
        var popup = new TestPopup
        {
            ViewModel = null
        };

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
        {
            popup.TriggerOpened();
            popup.TriggerClosed(wasDismissedByTappingOutside: false);
        });

        Assert.Null(exception);
    }

    #endregion

    #region Integration with ViewModelBase

    [Fact]
    public void ViewModelBase_Should_Have_All_Required_Lifecycle_Methods()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TestViewModel>>();
        var mockNavigationService = new Mock<INavigationService>();
        var viewModel = new TestViewModel(mockLogger.Object, mockNavigationService.Object);

        // Act & Assert - verify all lifecycle methods exist
        var viewModelType = viewModel.GetType();
        
        Assert.NotNull(viewModelType.GetMethod("ViewAppearing"));
        Assert.NotNull(viewModelType.GetMethod("ViewAppeared"));
        Assert.NotNull(viewModelType.GetMethod("ViewDisappearing"));
        Assert.NotNull(viewModelType.GetMethod("ViewDisappeared"));
    }

    [Fact]
    public void Reflection_Based_Lifecycle_Should_Work_With_Any_ViewModel_Type()
    {
        // Arrange - use a minimal ViewModel that only has lifecycle methods
        var minimalViewModel = new MinimalViewModel();
        var page = new MinimalViewModelPage
        {
            ViewModel = minimalViewModel
        };

        // Act
        page.TriggerAppearing();
        page.TriggerDisappearing();

        // Assert
        Assert.True(minimalViewModel.ViewAppearingCalled);
        Assert.True(minimalViewModel.ViewAppearedCalled);
        Assert.True(minimalViewModel.ViewDisappearingCalled);
        Assert.True(minimalViewModel.ViewDisappearedCalled);
    }

    #endregion

    #region Test Helpers

    /// <summary>
    /// Test page that exposes lifecycle methods for testing.
    /// </summary>
    private class TestPage : BasePage<TestViewModel>
    {
        public void TriggerAppearing() => OnAppearing();
        public void TriggerDisappearing() => OnDisappearing();
    }

    /// <summary>
    /// Test popup that exposes lifecycle events for testing.
    /// </summary>
    private class TestPopup : BasePopup<TestViewModel>
    {
        public void TriggerOpened()
        {
            // Simulate the Opened event
            var eventArgs = new CommunityToolkit.Maui.Core.PopupOpenedEventArgs();
            var openedEvent = typeof(Popup).GetEvent("Opened");
            var openedDelegate = Delegate.CreateDelegate(
                openedEvent!.EventHandlerType!,
                this,
                GetType().BaseType!.GetMethod("OnPopupOpened", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!);
            openedDelegate.DynamicInvoke(this, eventArgs);
        }

        public void TriggerClosed(bool wasDismissedByTappingOutside)
        {
            // Simulate the Closed event
            var eventArgs = new CommunityToolkit.Maui.Core.PopupClosedEventArgs(null, wasDismissedByTappingOutside);
            var closedEvent = typeof(Popup).GetEvent("Closed");
            var closedDelegate = Delegate.CreateDelegate(
                closedEvent!.EventHandlerType!,
                this,
                GetType().BaseType!.GetMethod("OnPopupClosed",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!);
            closedDelegate.DynamicInvoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Test ViewModel that tracks lifecycle method calls.
    /// </summary>
    private class TestViewModel : ViewModelBase
    {
        public bool ViewAppearingCalled { get; private set; }
        public bool ViewAppearedCalled { get; private set; }
        public bool ViewDisappearingCalled { get; private set; }
        public bool ViewDisappearedCalled { get; private set; }
        
        public List<string> LifecycleCallOrder { get; } = new();

        public TestViewModel(ILogger logger, INavigationService navigationService)
            : base(logger, navigationService)
        {
        }

        public override string Name => "TestViewModel";

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            ViewAppearingCalled = true;
            LifecycleCallOrder.Add("ViewAppearing");
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            ViewAppearedCalled = true;
            LifecycleCallOrder.Add("ViewAppeared");
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            ViewDisappearingCalled = true;
            LifecycleCallOrder.Add("ViewDisappearing");
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            ViewDisappearedCalled = true;
            LifecycleCallOrder.Add("ViewDisappeared");
        }
    }

    /// <summary>
    /// Minimal ViewModel without inheriting from ViewModelBase.
    /// Tests that reflection-based lifecycle works with any class.
    /// </summary>
    private class MinimalViewModel
    {
        public bool ViewAppearingCalled { get; private set; }
        public bool ViewAppearedCalled { get; private set; }
        public bool ViewDisappearingCalled { get; private set; }
        public bool ViewDisappearedCalled { get; private set; }

        public void ViewAppearing() => ViewAppearingCalled = true;
        public void ViewAppeared() => ViewAppearedCalled = true;
        public void ViewDisappearing() => ViewDisappearingCalled = true;
        public void ViewDisappeared() => ViewDisappearedCalled = true;
    }

    private class MinimalViewModelPage : BasePage<MinimalViewModel>
    {
        public void TriggerAppearing() => OnAppearing();
        public void TriggerDisappearing() => OnDisappearing();
    }

    #endregion
}
