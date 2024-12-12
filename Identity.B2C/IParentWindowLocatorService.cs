namespace B2C
{
    /// <summary> Simple platform specific service that is responsible for locating a  </summary>
    public interface IParentWindowLocatorService
    {
        object GetCurrentParentWindow();
    }

#if ANDROID
    public class AndroidParentWindowLocatorService : IParentWindowLocatorService
    {
        public AndroidParentWindowLocatorService()
        {

        }

        public object GetCurrentParentWindow()
        {
            //var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

            var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current;
            return activity;
        }
    }
#endif

}
