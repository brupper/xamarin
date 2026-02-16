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
            // Get the current Activity
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            

            // Or wait for it if needed during startup
            // var activity = await Platform.WaitForActivityAsync();
                
            return activity;
        }
    }
#endif

}
