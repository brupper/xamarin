//using Brupper.Forms.Platforms.Windows.Services;
//using Brupper.Forms.Presenters;
//using Brupper.Forms.Services;
//using Brupper.Forms.Services.Interfaces;
//using MvvmCross;
//using MvvmCross.Commands;
//using MvvmCross.Forms.Platforms.Wpf.Core;
//using MvvmCross.Forms.Presenters;
//using MvvmCross.IoC;
//using Microsoft.Extensions.Logging;
//using MvvmCross.Platforms.Wpf.Presenters;
//using MvvmCross.ViewModels;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Windows.Controls;

//namespace Brupper.Forms.Platforms.Windows
//{
//    public class Setup<TApplication, TFormsApplication> : MvxFormsWpfSetup<TApplication, TFormsApplication>
//        where TApplication : class, IMvxApplication, new()
//        where TFormsApplication : Xamarin.Forms.Application, new()
//    {
//        public Setup()
//        { }

//        protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
//        {
//            Mvx.IoCProvider.RegisterType<IMvxCommandHelper, MvxStrongCommandHelper>(); // https://github.com/MvvmCross/MvvmCross/issues/3689

//            base.InitializeFirstChance(iocProvider);

//            var platformInformationService = new PlatformInformationService();
//            Mvx.IoCProvider.RegisterSingleton<IPlatformInformationService>(platformInformationService);
//            Mvx.IoCProvider.RegisterCrossServices();

//            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPermissionHelper, PermissionHelper>();
//            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IOutputRendererServices, OutputRendererServices>();
//            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IImageResizer, ImageResizer>();

//            //if (App is IApplicationStateListener stateListener)
//            //    Mvx.IoCProvider.RegisterSingleton(stateListener);
//        }

//        protected override ILoggerProvider CreateLogProvider() => new AppCenterTrace(base.CreateLogProvider());

//        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
//        {
//            var formsPagePresenter = new PagePresenter(viewPresenter);
//            Mvx.IoCProvider.RegisterSingleton<IMvxFormsPagePresenter>(formsPagePresenter);
//            return formsPagePresenter;
//        }

//        protected override IMvxWpfViewPresenter CreateViewPresenter(ContentControl root)
//        {
//            // DO SOMETHING BEFORE Xamarin.Forms.Forms.Initialize called:

//            //UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
//            //Mvx.IoCProvider.RegisterSingleton((IApplicationStateListener)ApplicationContext);
//            //ZXing.Net.Mobile.Forms.iOS.Platform.Init();
//            //FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

//            Xamarin.Forms.Forms.SetFlags(new[]{
//                "Brush_Experimental",
//                "SwipeView_Experimental",
//                "CarouselView_Experimental",
//                "IndicatorView_Experimental",  // https://devblogs.microsoft.com/xamarin/xamarin-forms-4-4/
//            });

//            return base.CreateViewPresenter(root);
//        }

//        //public override IEnumerable<Assembly> GetViewAssemblies()
//        //{
//        //    var list = new List<Assembly>();
//        //    list.AddRange(base.GetViewAssemblies());
//        //    list.Add(typeof(Brupper.Forms.Pages.Base.MvxPopupPage<>).Assembly);

//        //    return list.ToArray();
//        //}

//        public override IEnumerable<Assembly> GetViewModelAssemblies()
//        {
//            var list = new List<Assembly>();
//            list.AddRange(base.GetViewModelAssemblies());
//            list.Add(typeof(Brupper.ViewModels.Popups.MvxPopupViewModel).Assembly);

//            return list.ToArray();
//        }
//    }
//}