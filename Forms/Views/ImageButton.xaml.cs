using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;

namespace Brupper.Forms.Views.Controls
{
    public partial class ImageButton : Frame
    {
        #region Private Fields

        private const string VisualStatePressed = "Pressed";
        private const string VisualStateCompleted = "Completed";

        #endregion

        #region Constructors

        public ImageButton()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private bool HasCompletedVisualState
        {
            get => VisualStateManager.GetVisualStateGroups(this).SelectMany(x => x.States).Any(state => state.Name == VisualStateCompleted);
        }

        #endregion

        #region CommandProperty

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandProperty =
                BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ImageButton));

        #endregion

        #region CommandParameterProperty

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty =
                BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ImageButton));

        #endregion

        #region TextProperty

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
                BindableProperty.Create(nameof(Text), typeof(string), typeof(ImageButton), string.Empty, propertyChanged: OnTextChanged);

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ImageButton)bindable).OnTextChanged((string)oldValue, (string)newValue);
        }

        private void OnTextChanged(string oldValue, string newValue)
        {
            UpdateIconPosition(newValue);
        }

        #endregion

        #region FontSizeProperty

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty =
                BindableProperty.Create(nameof(FontSize), typeof(double), typeof(ImageButton), 14d);

        #endregion

        #region TextColorProperty

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
                BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ImageButton), Color.Default);

        #endregion

        #region ImageSourceProperty

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty ImageSourceProperty =
                BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(ImageButton));

        #endregion

        #region IsBusyProperty

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly BindableProperty IsBusyProperty =
                BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(ImageButton), false, propertyChanged: OnIsBusyChanged);

        private static void OnIsBusyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ImageButton)bindable).OnIsBusyChanged((bool)oldValue, (bool)newValue);
        }

        private async void OnIsBusyChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                Button.Text = null;
                UpdateIconPosition(null);

                LoadingIndicatorLayout.IsVisible = true;
            }
            else if (oldValue && !newValue)
            {
                LoadingIndicatorLayout.IsVisible = false;
                VisualStateManager.GoToState(this, VisualStateCompleted);

                if (HasCompletedVisualState)
                    await Task.Delay(600);

                VisualStateManager.GoToState(this, VisualStateManager.CommonStates.Normal);

                UpdateIconPosition(Text);
                Button.SetBinding(Button.TextProperty, new Binding(nameof(Text), source: this));
            }
        }

        #endregion

        #region Private Methods

        private void OnButtonPressed(object sender, EventArgs e)
        {
            VisualStateManager.GoToState((VisualElement)sender, VisualStatePressed);
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState((VisualElement)sender, VisualStateManager.CommonStates.Normal);

            if (Command?.CanExecute(CommandParameter) ?? false)
                Command.Execute(CommandParameter);
        }

        private void UpdateIconPosition(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText))
            {
                IconImage.HorizontalOptions = LayoutOptions.Center;
                IconImage.Margin = new Thickness(0);
            }
            else
            {
                IconImage.HorizontalOptions = LayoutOptions.End;
                IconImage.Margin = new Thickness(0, 0, 18, 0);
            }
        }

        #endregion
    }
}
