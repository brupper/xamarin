using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms.Views
{
    public enum SuggestionPlacement { Bottom, Top }

    /// <summary> https://github.com/DottorPagliaccius/Xamarin-Custom-Controls/tree/master/src/Xamarin.CustomControls.AutoCompleteView </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoCompleteView
    {
        private Layout<View> _optionalSuggestionsPanelContainer;
        private SuggestionPlacement _suggestionPlacement = SuggestionPlacement.Bottom;

        private PropertyInfo _searchMemberCachePropertyInfo;

        private ObservableCollection<object> _availableSuggestions;

        public event EventHandler OnSuggestionOpen;
        public event EventHandler OnSuggestionClose;

        public static readonly BindableProperty EntryStyleProperty = BindableProperty.Create(nameof(EntryStyle), typeof(Style), typeof(AutoCompleteView), null);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AutoCompleteView), string.Empty);
        public static readonly BindableProperty FilterTextProperty = BindableProperty.Create(nameof(FilterText), typeof(string), typeof(AutoCompleteView), string.Empty, BindingMode.TwoWay);
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(AutoCompleteView), string.Empty);
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(AutoCompleteView), default(DataTemplate));
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(AutoCompleteView), new List<object>());
        public static readonly BindableProperty EmptyTextProperty = BindableProperty.Create(nameof(EmptyText), typeof(string), typeof(AutoCompleteView), string.Empty);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(AutoCompleteView), null, BindingMode.TwoWay);
        public static readonly BindableProperty SelectedItemCommandProperty = BindableProperty.Create(nameof(SelectedItemCommand), typeof(ICommand), typeof(AutoCompleteView), default(ICommand));

        public static readonly BindableProperty SuggestionBackgroundColorProperty = BindableProperty.Create(nameof(SuggestionBackgroundColor), typeof(Color), typeof(AutoCompleteView), Color.Transparent);
        public static readonly BindableProperty SuggestionBorderColorProperty = BindableProperty.Create(nameof(SuggestionBorderColor), typeof(Color), typeof(AutoCompleteView), Color.Silver);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(AutoCompleteView), Color.Black);
        public static readonly BindableProperty PlaceholderTextColorProperty = BindableProperty.Create(nameof(PlaceholderTextColor), typeof(Color), typeof(AutoCompleteView), Color.Silver);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(AutoCompleteView), Font.Default.FontSize);

        public static readonly BindableProperty SearchMemberProperty = BindableProperty.Create(nameof(SearchMember), typeof(string), typeof(AutoCompleteView), string.Empty);

        public static readonly BindableProperty MinimumCharacterNumberBeforSearchStartsProperty = BindableProperty.Create(nameof(MinimumCharacterNumberBeforSearchStarts), typeof(int), typeof(AutoCompleteView), 0);

        //public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(AutoCompleteView), Color.Silver);
        //public static readonly BindableProperty SeparatorHeightProperty = BindableProperty.Create(nameof(SeparatorHeight), typeof(double), typeof(AutoCompleteView), 1.5d);

        public static readonly BindableProperty EntryLineColorProperty = BindableProperty.Create(nameof(EntryLineColor), typeof(Color), typeof(AutoCompleteView), Color.Black);
        public static readonly BindableProperty EntryLineHeightProperty = BindableProperty.Create(nameof(EntryLineHeight), typeof(double), typeof(AutoCompleteView), 1d);

        public Style EntryStyle
        {
            get => (Style)GetValue(EntryStyleProperty);
            set => SetValue(EntryStyleProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set { SetValue(ItemsSourceProperty, value); OnPropertyChanged(); }
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public string EmptyText
        {
            get => (string)GetValue(EmptyTextProperty);
            set => SetValue(EmptyTextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color SuggestionBackgroundColor
        {
            get => (Color)GetValue(SuggestionBackgroundColorProperty);
            set => SetValue(SuggestionBackgroundColorProperty, value);
        }

        public Color SuggestionBorderColor
        {
            get => (Color)GetValue(SuggestionBorderColorProperty);
            set => SetValue(SuggestionBorderColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public Color PlaceholderTextColor
        {
            get => (Color)GetValue(PlaceholderTextColorProperty);
            set => SetValue(PlaceholderTextColorProperty, value);
        }

        public ICommand SelectedItemCommand
        {
            get => (ICommand)GetValue(SelectedItemCommandProperty);
            set => SetValue(SelectedItemCommandProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string SearchMember
        {
            get => (string)GetValue(SearchMemberProperty);
            set => SetValue(SearchMemberProperty, value);
        }

        public int MinimumCharacterNumberBeforSearchStarts
        {
            get => (int)GetValue(MinimumCharacterNumberBeforSearchStartsProperty);
            set => SetValue(MinimumCharacterNumberBeforSearchStartsProperty, value);
        }

        //public Color SeparatorColor
        //{
        //    get => (Color)GetValue(SeparatorColorProperty);
        //    set => SetValue(SeparatorColorProperty, value);
        //}

        //public double SeparatorHeight
        //{
        //    get => (double)GetValue(SeparatorHeightProperty);
        //    set => SetValue(SeparatorHeightProperty, value);
        //}

        //public bool ShowSeparator
        //{
        //    get => SuggestedItemsRepeaterView.ShowSeparator;
        //    set => SuggestedItemsRepeaterView.ShowSeparator = value;
        //}

        public bool OpenOnFocus { get; set; }
        public int MaxResults { get; set; }

        public SuggestionPlacement SuggestionPlacement
        {
            get => _suggestionPlacement;
            set
            {
                _suggestionPlacement = value;

                OnPropertyChanged();
                PlaceSuggestionPanel();
            }
        }

        public bool ShowEntryLine
        {
            get => EntryLine.IsVisible;

            set => EntryLine.IsVisible = value;
        }

        public Color EntryLineColor
        {
            get => (Color)GetValue(EntryLineColorProperty);
            set => SetValue(EntryLineColorProperty, value);
        }

        public double EntryLineHeight
        {
            get => (double)GetValue(EntryLineHeightProperty);
            set => SetValue(EntryLineHeightProperty, value);
        }

        public Layout<View> OptionalSuggestionsPanelContainer
        {
            get => _optionalSuggestionsPanelContainer;
            set
            {
                _optionalSuggestionsPanelContainer = value;

                if (value == null)
                    return;

                OnPropertyChanged();
                PlaceSuggestionPanel();
            }
        }

        public AutoCompleteView()
        {
            InitializeComponent();

            _availableSuggestions = new ObservableCollection<object>();

            //SuggestedItemsRepeaterView.SelectedItemCommand = new Command(SuggestedRepeaterItemSelected);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == TextProperty.PropertyName && SelectedItem == null)
            {
                MainEntry.Text = Text;
            }

            if (propertyName == SelectedItemProperty.PropertyName)
            {
                if (SelectedItem != null)
                {
                    var propertyInfo = GetSearchMember(SelectedItem.GetType());

                    var selectedItem = ItemsSource.Cast<object>().SingleOrDefault(x => propertyInfo.GetValue(x).ToString() == propertyInfo.GetValue(SelectedItem).ToString());

                    if (selectedItem != null)
                    {
                        try
                        {
                            MainEntry.TextChanged -= SearchText_TextChanged;

                            MainEntry.Text = propertyInfo.GetValue(SelectedItem).ToString();
                        }
                        finally
                        {
                            MainEntry.TextChanged -= SearchText_TextChanged;
                        }

                        FilterSuggestions(MainEntry.Text, false);

                        MainEntry.TextColor = TextColor;
                    }
                    else
                    {
                        MainEntry.Text = Text;
                        MainEntry.TextColor = PlaceholderTextColor;
                    }
                }
                else
                {
                    MainEntry.Text = Text;
                    MainEntry.TextColor = PlaceholderTextColor;
                }
            }

            if (propertyName == SearchMemberProperty.PropertyName)
            {
                _searchMemberCachePropertyInfo = null;
            }

            if (propertyName == ItemTemplateProperty.PropertyName)
            {
                BindableLayout.SetItemTemplate(SuggestedItemsRepeaterView, ItemTemplate);
                // todo SuggestedItemsRepeaterView.ItemTemplate = ItemTemplate;
            }

            if (propertyName == SuggestionBackgroundColorProperty.PropertyName)
            {
                SuggestedItemsOuterContainer.BackgroundColor = SuggestionBackgroundColor;
            }

            if (propertyName == SuggestionBorderColorProperty.PropertyName)
            {
                SuggestedItemsOuterContainer.BorderColor = SuggestionBorderColor;
            }

            //if (propertyName == EmptyTextProperty.PropertyName)
            //{
            //    SuggestedItemsRepeaterView.EmptyText = EmptyText;
            //}

            //if (propertyName == SeparatorColorProperty.PropertyName)
            //{
            //    SuggestedItemsRepeaterView.SeparatorColor = SeparatorColor;
            //}

            //if (propertyName == SeparatorHeightProperty.PropertyName)
            //{
            //    SuggestedItemsRepeaterView.SeparatorHeight = SeparatorHeight;
            //}

            if (propertyName == EntryLineColorProperty.PropertyName)
            {
                EntryLine.Color = EntryLineColor;
            }

            //if (propertyName == SeparatorHeightProperty.PropertyName)
            //{
            //    EntryLine.HeightRequest = EntryLineHeight;
            //}
        }

        private void SearchText_FocusChanged(object sender, FocusEventArgs e)
        {
            HandleFocus(e.IsFocused);
        }

        private void HandleFocus(bool isFocused)
        {
            MainEntry.TextChanged -= SearchText_TextChanged;

            try
            {
                if (isFocused)
                {
                    if (string.Equals(MainEntry.Text, Text, StringComparison.OrdinalIgnoreCase))
                    {
                        MainEntry.Text = string.Empty;
                        MainEntry.TextColor = TextColor;
                    }

                    if (OpenOnFocus)
                    {
                        FilterSuggestions(MainEntry.Text);
                    }
                }
                else
                {
                    var items = ItemsSource.Cast<object>();

                    if (MainEntry.Text?.Length == 0 || (items.Any() && !items.Any(x => string.Equals(GetSearchMember(items.First().GetType()).GetValue(x).ToString(), MainEntry.Text, StringComparison.Ordinal))))
                    {
                        //MainEntry.Text = Text; // Filter text miatt ne legyen torolve a default value
                        //MainEntry.TextColor = PlaceholderTextColor;
                    }
                    else
                        MainEntry.TextColor = TextColor;

                    ShowHideListbox(false);
                }
            }
            finally
            {
                MainEntry.TextChanged += SearchText_TextChanged;
            }
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.Equals(MainEntry.Text, Text, StringComparison.OrdinalIgnoreCase))
            {
                FilterText = Text;

                if (_availableSuggestions.Any())
                {
                    _availableSuggestions.Clear();

                    ShowHideListbox(false);
                }

                return;
            }

            FilterSuggestions(MainEntry.Text);
        }

        private void FilterSuggestions(string text, bool openSuggestionPanel = true)
        {
            var filteredSuggestions = ItemsSource.Cast<object>();

            if (text?.Length > 0 && filteredSuggestions.Any())
            {
                var property = GetSearchMember(filteredSuggestions.First().GetType());

                if (property == null)
                    throw new MemberNotFoundException($"There's no corrisponding property the matches SearchMember value '{SearchMember}'");

                if (property.PropertyType != typeof(string))
                    throw new SearchMemberPropertyTypeException($"Property '{SearchMember}' must be of type string");

                filteredSuggestions = filteredSuggestions.Where(x => property.GetValue(x).ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).OrderByDescending(x => property.GetValue(x).ToString());
            }

            _availableSuggestions = new ObservableCollection<object>(MaxResults > 0 ? filteredSuggestions.Take(MaxResults) : filteredSuggestions);

            if (!_availableSuggestions.Any())
            {
                ShowHideListbox(false, true);
                return;
            }

            ShowHideListbox(openSuggestionPanel);
        }

        private PropertyInfo GetSearchMember(Type type)
        {
            if (_searchMemberCachePropertyInfo != null)
                return _searchMemberCachePropertyInfo;

            if (string.IsNullOrEmpty(SearchMember))
                throw new MemberNotFoundException("You must specify SearchMember property");

            _searchMemberCachePropertyInfo = type.GetRuntimeProperty(SearchMember);

            if (_searchMemberCachePropertyInfo == null)
                throw new MemberNotFoundException($"There's no corrisponding property the matches SearchMember value '{SearchMember}'");

            if (_searchMemberCachePropertyInfo.PropertyType != typeof(string))
                throw new SearchMemberPropertyTypeException($"Property '{SearchMember}' must be of type string");

            return _searchMemberCachePropertyInfo;
        }

        private void SuggestedRepeaterItemSelected(object selectedItem)
        {
            MainEntry.Text = GetSelectedText(selectedItem);
            MainEntry.TextColor = TextColor;

            ShowHideListbox(false);

            _availableSuggestions.Clear();

            SelectedItem = selectedItem;

            SelectedItemCommand?.Execute(selectedItem);
        }

        private string GetSelectedText(object selectedItem)
        {
            var property = selectedItem.GetType().GetRuntimeProperty(SearchMember);

            if (property == null)
                throw new MemberNotFoundException($"There's no corrisponding property the matches DisplayMember value '{SearchMember}'");

            return property.GetValue(selectedItem).ToString();
        }

        private void PlaceSuggestionPanel()
        {
            if (OptionalSuggestionsPanelContainer == null)
            {
                SuggestedItemsContainerTop.IsVisible = false;
                SuggestedItemsContainerBottom.IsVisible = false;

                if (SuggestionPlacement == SuggestionPlacement.Bottom)
                {
                    if (SuggestedItemsContainerTop.Children.Any())
                    {
                        var suggestionPanel = SuggestedItemsContainerTop.Children.First();

                        SuggestedItemsContainerTop.Children.Remove(suggestionPanel);
                        SuggestedItemsContainerBottom.Children.Add(suggestionPanel);
                    }
                }
                else
                {
                    if (SuggestedItemsContainerBottom.Children.Any())
                    {
                        var suggestionPanel = SuggestedItemsContainerBottom.Children.First();

                        SuggestedItemsContainerBottom.Children.Remove(suggestionPanel);
                        SuggestedItemsContainerTop.Children.Add(suggestionPanel);
                    }
                }
            }
            else
            {
                if (SuggestedItemsContainerTop.Children.Any())
                {
                    var suggestionPanel = SuggestedItemsContainerTop.Children.First();

                    SuggestedItemsContainerTop.Children.Remove(suggestionPanel);
                    OptionalSuggestionsPanelContainer.Children.Add(suggestionPanel);
                }

                if (SuggestedItemsContainerBottom.Children.Any())
                {
                    var suggestionPanel = SuggestedItemsContainerBottom.Children.First();

                    SuggestedItemsContainerBottom.Children.Remove(suggestionPanel);
                    OptionalSuggestionsPanelContainer.Children.Add(suggestionPanel);
                }
            }
        }

        private void ShowHideListbox(bool show, bool skipUnFocus = false)
        {
            if (MinimumCharacterNumberBeforSearchStarts >= MainEntry.Text?.Length)
            {
                return;
            }

            if (show)
            {
                BindableLayout.SetItemsSource(SuggestedItemsRepeaterView, _availableSuggestions);
                //TODO : SuggestedItemsRepeaterView.ItemsSource = _availableSuggestions;

                if (!SuggestedItemsContainerTop.IsVisible && !SuggestedItemsContainerBottom.IsVisible)
                    OnSuggestionOpen?.Invoke(this, new EventArgs());
            }
            else
            {
                if (SuggestedItemsContainerTop.IsVisible || SuggestedItemsContainerBottom.IsVisible)
                {
                    if (!skipUnFocus)
                    {
                        MainEntry.Unfocus();
                        Unfocus();
                    }

                    OnSuggestionClose?.Invoke(this, new EventArgs());
                }
            }

            if (SuggestionPlacement == SuggestionPlacement.Top)
                SuggestedItemsContainerTop.IsVisible = show;
            else
                SuggestedItemsContainerBottom.IsVisible = show;
        }
    }

    public class MemberNotFoundException : Exception
    {

        public MemberNotFoundException()
        {
        }

        public MemberNotFoundException(string message) : base(message)
        {
        }

        public MemberNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class SearchMemberPropertyTypeException : Exception
    {
        public SearchMemberPropertyTypeException()
        {
        }

        public SearchMemberPropertyTypeException(string message) : base(message)
        {
        }

        public SearchMemberPropertyTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}