﻿using Microsoft.AppCenter.Crashes;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Popups
{
    public class LoadingViewModelParameter : Core.Models.NotifyPropertyChanged
    {
        private string message;
        private bool canBeCancelled;

        public bool CanBeCancelled { get => canBeCancelled; set => SetProperty(ref canBeCancelled, value); }

        public string Message { get => message; set => SetProperty(ref message, value); }

        public Action Cancel { get; set; }

        public Task RunningTask { get; set; }
            = Task.CompletedTask;

        public CancellationToken Token { get; set; }
            = CancellationToken.None;

        public int TimeoutInMilliseconds { get; set; }
            = 5000;
    }

    public abstract class BaseLoadingViewModel : MvxPopupViewModel<LoadingViewModelParameter>
    {
        private IMvxCommand cancelCommand;
        private bool isRunning = true;
        protected LoadingViewModelParameter context = new LoadingViewModelParameter();

        #region Constructor

        protected BaseLoadingViewModel(
            IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Overrides

        protected override Task ExecuteBackCommandAsync()
        {
            if (isRunning)
            {
                // can not be canceled while running, timeout will do that.
                return Task.CompletedTask;
            }

            return base.ExecuteBackCommandAsync();
        }

        public override void Prepare(LoadingViewModelParameter parameter)
        {
            context = parameter;
        }

        public override Task Initialize()
        {
            Task.Run(async () =>
            {
                AlertViewModelParameter alertParam = null;
                var timeout = context.TimeoutInMilliseconds;
                var targetTask = context.RunningTask;

                await Task.Delay(25).ConfigureAwait(false);
                if (targetTask != null)
                {
                    isRunning = true;
                    try
                    {
                        if (await Task.WhenAny(targetTask, Task.Delay(timeout)).ConfigureAwait(false) == targetTask)
                        {
                            // task completed within timeout
                        }
                        else
                        {
                            // timeout logic
                        }

                        if (targetTask.IsFaulted)
                        {
                            alertParam = new AlertViewModelParameter
                            {
                                Message = $":'( \n\n{targetTask.Exception}",
                                HasException = true,
                            };
                        }
                    }
                    catch (Exception exception)
                    {
                        alertParam = new AlertViewModelParameter
                        {
                            Message = $":'( \n\n{exception}",
                            HasException = true,
                        };
                        Crashes.TrackError(exception);
                    }
                    finally
                    {
                        isRunning = false;
                    }
                }
                await Task.Delay(25).ConfigureAwait(false);

                await ExecuteBackCommandAsync().ConfigureAwait(false);

                if (alertParam != null)
                {
                    await navigationService.Navigate<AlertViewModel, AlertViewModelParameter>(alertParam).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
            return base.Initialize();
        }

        #endregion

        public IMvxCommand CancelCommand => cancelCommand ?? (cancelCommand = new MvxCommand(() =>
        {
            if ((context?.CanBeCancelled ?? false))
            {
                context.Cancel?.Invoke();
            }
        }));
    }
}
