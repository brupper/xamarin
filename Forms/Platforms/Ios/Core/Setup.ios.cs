using Brupper.Forms.Platforms.iOS.Services;
using Brupper.Forms.Presenters;
using Brupper.Forms.Services.Interfaces;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.Forms.Presenters;
using MvvmCross.IoC;
using MvvmCross.Logging;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Reflection;

namespace Brupper.Forms.Platforms.iOS
{
    public class Setup<TApplication, TFormsApplication> : MvxFormsIosSetup<TApplication, TFormsApplication>
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

        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter) => new PagePresenter(viewPresenter);

        protected override IMvxIosViewPresenter CreateViewPresenter()
        {
            //ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            //Rg.Plugins.Popup.Popup.Init(); => readme.txt-ben le van irva hogy nem a  Brupper intezi a hivast
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            return base.CreateViewPresenter();
        }

        // protected override Xamarin.Forms.Application CreateFormsApplication() => base.CreateFormsApplication();

        public override Xamarin.Forms.Application FormsApplication
        {
            get
            {
                // DO SOMETHING BEFORE Xamarin.Forms.Forms.Initialize called:

                // https://github.com/MvvmCross/MvvmCross/blob/c730b21dd350f187f32e22b01a289d22df09dd43/MvvmCross.Forms/Platforms/Ios/Core/MvxFormsIosSetup.cs
                // Experimantal FLAG-eket itt lehet iOS-en: 
                if (!Xamarin.Forms.Forms.IsInitialized)
                {
                    Xamarin.Forms.Forms.SetFlags(new[]{
                        "Brush_Experimental",
                        "SwipeView_Experimental",
                        "CarouselView_Experimental",
                        "IndicatorView_Experimental",  // https://devblogs.microsoft.com/xamarin/xamarin-forms-4-4/
                    });
                }

                return base.FormsApplication;
            }
        }

        //public override IEnumerable<Assembly> GetViewAssemblies()
        //{
        //    var list = new List<Assembly>();
        //    list.AddRange(base.GetViewAssemblies());
        //    list.Add(typeof(Brupper.Forms.Pages.Base.MvxPopupPage<>).Assembly);

        //    return list.ToArray();
        //}

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            var list = new List<Assembly>();
            list.AddRange(base.GetViewModelAssemblies());
            list.Add(typeof(Brupper.ViewModels.Popups.MvxPopupViewModel).Assembly);

            return list.ToArray();
        }
    }
}