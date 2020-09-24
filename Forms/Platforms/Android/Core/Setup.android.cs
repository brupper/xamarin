using Brupper.Forms.Platforms.Android.PlatformServices;
using Brupper.Forms.Platforms.Android.Services;
using Brupper.Forms.Presenters;
using Brupper.Forms.Services.Interfaces;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.Forms.Presenters;
using MvvmCross.IoC;
using MvvmCross.Logging;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;

namespace Brupper.Forms.Platforms.Android
{
    public class Setup<TApplication, TFormsApplication> : MvxFormsAndroidSetup<TApplication, TFormsApplication>
        where TApplication : class, IMvxApplication, new()
        where TFormsApplication : Xamarin.Forms.Application, new()
    {
        public Setup()
        { }

        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.RegisterType<IMvxCommandHelper, MvxStrongCommandHelper>(); // https://github.com/MvvmCross/MvvmCross/issues/3689

            base.InitializeFirstChance();

            var platformInformationService = new PlatformInformationService();
            Mvx.IoCProvider.RegisterSingleton<IPlatformInformationService>(platformInformationService);
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPermissionHelper, PermissionHelper>();
        }

        protected override IMvxLogProvider CreateLogProvider() => new AppCenterTrace(base.CreateLogProvider());

        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
        {
            var formsPagePresenter = new PagePresenter(viewPresenter);
            Mvx.IoCProvider.RegisterSingleton<IMvxFormsPagePresenter>(formsPagePresenter);
            return formsPagePresenter;
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            // DO SOMETHING BEFORE Xamarin.Forms.Forms.Initialize called:

            //UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            //Mvx.IoCProvider.RegisterSingleton((IApplicationStateListener)ApplicationContext);
            //ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            Xamarin.Forms.Forms.SetFlags(new[]{
                "Brush_Experimental",
                "SwipeView_Experimental",
                "CarouselView_Experimental",
                "IndicatorView_Experimental",  // https://devblogs.microsoft.com/xamarin/xamarin-forms-4-4/
            });

            return base.CreateViewPresenter();
        }
    }
}