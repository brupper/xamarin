using Brupper.Forms.ViewModels;
using MvvmCross;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class NavigationExtensions
{
    public static async Task<TResult> Navigate<TViewModel, TResult>(this ISupportBrupperViewModel viewModel)
        where TViewModel : IMvxViewModelResult<TResult>
    {
        viewModel.CanViewDestroy = false;

        TResult result = await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<TViewModel, TResult>();

        viewModel.CanViewDestroy = true;

        return result;
    }

    public static Task<bool> Navigate<TViewModel>(this ISupportBrupperViewModel viewModel)
        where TViewModel : IMvxViewModelResult<bool>
        => viewModel.Navigate<TViewModel, bool>();

    public static async Task<TResult> Navigate<TViewModel, TParam, TResult>(this ISupportBrupperViewModel viewModel, TParam parameter)
        where TViewModel : IMvxViewModel<TParam, TResult>
    {
        viewModel.CanViewDestroy = false;

        TResult result = await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<TViewModel, TParam, TResult>(parameter);

        viewModel.CanViewDestroy = true;

        return result;
    }
}
