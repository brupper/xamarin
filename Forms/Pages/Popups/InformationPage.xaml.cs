using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Brupper.Forms.Pages.Popups
{
    [DesignTimeVisible(false)]
    public partial class InformationPage
    {
        private bool stopRefreshTimer;

        public InformationPage()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                // Browser.Navigated does not work with local resources                
                 Device.BeginInvokeOnMainThread(async () => { try { if (Browser.IsVisible) await Browser.GetHeightOfWebViewAsync(); } catch { } });

                return !stopRefreshTimer; // True = Repeat again, False = Stop the timer
            });
        }

        protected override void OnDisappearing()
        {
            stopRefreshTimer = true;
            base.OnDisappearing();
        }
    }
}
