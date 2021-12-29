using Brupper.Forms.ViewModels;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Threading.Tasks;

public static class NavigationExtensions
{
    public class BooleanResult
    {
        public bool Success { get; set; }
    }

    public static async Task<TResult> Navigate<TViewModel, TResult>(this ISupportBrupperViewModel viewModel)
        where TViewModel : IMvxViewModelResult<TResult>
        where TResult : class
    {
        viewModel.CanViewDestroy = false;

        TResult result = await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<TViewModel, TResult>();

        viewModel.CanViewDestroy = true;

        return result;
    }

    //public static Task<TResult> Navigate<TViewModel, TResult>(this ISupportBrupperViewModel viewModel)
    //    where TViewModel : IMvxViewModelResult<TResult>
    //    where TResult : BooleanResult
    //    => viewModel.Navigate<TViewModel, TResult>();

    public static async Task<TResult> Navigate<TViewModel, TParam, TResult>(this ISupportBrupperViewModel viewModel, TParam parameter)
        where TViewModel : IMvxViewModel<TParam, TResult>
        where TResult : class
    {
        viewModel.CanViewDestroy = false;

        TResult result = await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<TViewModel, TParam, TResult>(parameter);

        viewModel.CanViewDestroy = true;

        return result;
    }

    public static async Task<bool> Close(this IMvxViewModel viewModel, IMvxViewModel viewModel2)
    {
        if (viewModel is ISupportBrupperViewModel support)
        {
            support.CanViewDestroy = true;
        }
        return await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Close(viewModel2);
    }

    public static Task<bool> Close<TResult>(this IMvxViewModelResult<TResult> viewModel, IMvxViewModelResult<TResult> viewModel2, TResult result)
        where TResult : class
    {
        if (viewModel is ISupportBrupperViewModel support)
        {
            support.CanViewDestroy = true;
        }
        return Mvx.IoCProvider.Resolve<IMvxNavigationService>().Close(viewModel2, result);
    }
}
