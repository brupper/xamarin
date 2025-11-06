using Brupper.Maui.Services;
using Microsoft.Extensions.Logging;

namespace Brupper.Maui.ViewModels;

/// <summary>
/// Base ViewModel with initialization parameter support.
/// </summary>
/// <typeparam name="TParam">The type of parameter passed during initialization.</typeparam>
public abstract partial class ViewModelBase<TParam> : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase{TParam}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected ViewModelBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Called when the ViewModel is being initialized with a parameter.
    /// </summary>
    /// <param name="parameter">The initialization parameter.</param>
    public virtual Task InitializeAsync(TParam parameter)
    {
        Logger.LogInformation("Initializing {ViewModelName} with parameter of type {ParameterType}",
            Name, typeof(TParam).Name);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Base ViewModel with initialization parameter and result support.
/// </summary>
/// <typeparam name="TParam">The type of parameter passed during initialization.</typeparam>
/// <typeparam name="TResult">The type of result returned from this ViewModel.</typeparam>
public abstract partial class ViewModelBase<TParam, TResult> : ViewModelBase
{
    private TaskCompletionSource<TResult?>? closeCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase{TParam, TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected ViewModelBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Called when the ViewModel is being initialized with a parameter.
    /// </summary>
    /// <param name="parameter">The initialization parameter.</param>
    public virtual Task InitializeAsync(TParam parameter)
    {
        Logger.LogInformation("Initializing {ViewModelName} with parameter of type {ParameterType}",
            Name, typeof(TParam).Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the ViewModel with a result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    protected virtual Task CloseAsync(TResult? result)
    {
        closeCompletionSource?.TrySetResult(result);
        return NavigationService.PopAsync();
    }

    /// <summary>
    /// Gets a task that completes when the ViewModel is closed, returning the result.
    /// </summary>
    /// <returns>A task that represents the result of the ViewModel.</returns>
    public Task<TResult?> GetResultAsync()
    {
        closeCompletionSource ??= new TaskCompletionSource<TResult?>();
        return closeCompletionSource.Task;
    }
}

/// <summary>
/// Base ViewModel with result support (no parameter).
/// </summary>
/// <typeparam name="TResult">The type of result returned from this ViewModel.</typeparam>
public abstract partial class ViewModelResultBase<TResult> : ViewModelBase
{
    private TaskCompletionSource<TResult?>? closeCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelResultBase{TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="navigationService">The navigation service.</param>
    protected ViewModelResultBase(ILogger logger, INavigationService navigationService)
        : base(logger, navigationService)
    {
    }

    /// <summary>
    /// Closes the ViewModel with a result.
    /// </summary>
    /// <param name="result">The result to return.</param>
    protected virtual Task CloseAsync(TResult? result)
    {
        closeCompletionSource?.TrySetResult(result);
        return NavigationService.PopAsync();
    }

    /// <summary>
    /// Gets a task that completes when the ViewModel is closed, returning the result.
    /// </summary>
    /// <returns>A task that represents the result of the ViewModel.</returns>
    public Task<TResult?> GetResultAsync()
    {
        closeCompletionSource ??= new TaskCompletionSource<TResult?>();
        return closeCompletionSource.Task;
    }
}
