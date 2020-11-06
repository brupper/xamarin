Android:
Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IToastNotificationService, AndroidToastNotificationService>();

Brupper.Push.Platforms.Android.Services.AndroidToastNotificationService.Init(this);
Plugin.Toasts.ToastNotification.Init(this);

iOS:
Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IToastNotificationService, AppleToastNotificationService>();

protected override IMvxIosViewPresenter CreateViewPresenter()
	AppleToastNotificationService.Init();
	Plugin.Toasts.ToastNotification.Init();

Based on:

- https://github.com/Azure/azure-notificationhubs-xamarin
- https://github.com/Azure/azure-notificationhubs-android
- https://github.com/Azure/azure-notificationhubs-ios