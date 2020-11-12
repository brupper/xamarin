using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Brupper.Forms.Views
{
    /// <summary> https://github.com/TBertuzzi/Xamarin.Forms.PinchZoomImage/blob/master/Xamarin.Forms.PinchZoomImage/PinchZoom.cs </summary>
    public class PinchToZoomContainer : ContentView
    {
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;

        public double CurrentScale { get; private set; } = MIN_SCALE;
        public double StartScale { get; private set; } = MIN_SCALE;
        public double XOffset { get; set; } = 0;
        public double YOffset { get; set; } = 0;

        public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.NumberOfTapsRequired = 2;
            tapGesture.Tapped += DoubleTapped;
            GestureRecognizers.Add(tapGesture);
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                StartScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }

            if (e.Status == GestureStatus.Running)
            {
                CurrentScale += (e.Scale - 1) * StartScale;
                CurrentScale = Math.Max(1, CurrentScale);

                double renderedX = Content.X + XOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * StartScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                double renderedY = Content.Y + YOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * StartScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                double targetX = XOffset - (originX * Content.Width) * (CurrentScale - StartScale);
                double targetY = YOffset - (originY * Content.Height) * (CurrentScale - StartScale);

                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (CurrentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (CurrentScale - 1)));

                Content.Scale = CurrentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                XOffset = Content.TranslationX;
                YOffset = Content.TranslationY;
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Content.Scale == 1)
            {
                return;
            }

            switch (e.StatusType)
            {
                case GestureStatus.Running:

                    double newX = (e.TotalX * Scale) + XOffset;
                    double newY = (e.TotalY * Scale) + YOffset;

                    double width = (Content.Width * Content.Scale);
                    double height = (Content.Height * Content.Scale);

                    bool canMoveX = width > Application.Current.MainPage.Width;
                    bool canMoveY = height > Application.Current.MainPage.Height;

                    if (canMoveX)
                    {
                        double minX = (width - (Application.Current.MainPage.Width / 2)) * -1;
                        double maxX = Math.Min(Application.Current.MainPage.Width / 2, width / 2);

                        if (newX < minX)
                        {
                            newX = minX;
                        }

                        if (newX > maxX)
                        {
                            newX = maxX;
                        }
                    }
                    else
                    {
                        newX = 0;
                    }

                    if (canMoveY)
                    {
                        double minY = (height - (Application.Current.MainPage.Height / 2)) * -1;
                        double maxY = Math.Min(Application.Current.MainPage.Width / 2, height / 2);

                        if (newY < minY)
                        {
                            newY = minY;
                        }

                        if (newY > maxY)
                        {
                            newY = maxY;
                        }
                    }
                    else
                    {
                        newY = 0;
                    }

                    Content.TranslationX = newX;
                    Content.TranslationY = newY;
                    break;
                case GestureStatus.Completed:
                    XOffset = Content.TranslationX;
                    YOffset = Content.TranslationY;
                    break;
            }
        }

        public async void DoubleTapped(object sender, EventArgs e)
        {
            //if (Content.Scale >= MAX_SCALE)
            //{
            //    RestoreScaleValues();
            //    return;
            //}

            double multiplicator = Math.Pow(2, 1.0 / 10.0);
            StartScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;

            for (int i = 0; i < 10; i++)
            {
                CurrentScale *= multiplicator;
                double renderedX = Content.X + XOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * StartScale);
                double originX = (0.5 - deltaX) * deltaWidth;

                double renderedY = Content.Y + YOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * StartScale);
                double originY = (0.5 - deltaY) * deltaHeight;

                double targetX = XOffset - (originX * Content.Width) * (CurrentScale - StartScale);
                double targetY = YOffset - (originY * Content.Height) * (CurrentScale - StartScale);

                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (CurrentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (CurrentScale - 1)));

                Content.Scale = CurrentScale;
                await Task.Delay(10);
            }

            XOffset = Content.TranslationX;
            YOffset = Content.TranslationY;
        }

        private void RestoreScaleValues()
        {
            Content.ScaleTo(MIN_SCALE, 250, Easing.CubicInOut);
            Content.TranslateTo(0, 0, 250, Easing.CubicInOut);

            StartScale = CurrentScale = 1;

            XOffset = Content.TranslationX = 0;
            YOffset = Content.TranslationY = 0;
        }
    }
}
