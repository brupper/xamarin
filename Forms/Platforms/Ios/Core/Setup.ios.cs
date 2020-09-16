using Brupper.Forms.Platforms.iOS.Services;
using Brupper.Forms.Presenters;
using Brupper.Forms.Services.Interfaces;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.Forms.Presenters;
using MvvmCross.Logging;
using MvvmCross.ViewModels;

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
        }

        protected override IMvxLogProvider CreateLogProvider() => new AppCenterTrace(base.CreateLogProvider());

        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
        {
            return new PagePresenter(viewPresenter);
        }
    }
}