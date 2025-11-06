using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Widget;
using Brupper.Forms.Effects;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Reflection;

[assembly: ExportEffect(typeof(Brupper.Forms.Platforms.Android.Effects.BorderPlatformEffect), nameof(Border))]
namespace Brupper.Forms.Platforms.Android.Effects
{
    [Preserve(AllMembers = true)]
    public class BorderPlatformEffect : EffectBase
    {
        global::Android.Views.View _view;
        GradientDrawable _border;
        global::Android.Graphics.Color _color;
        int _width;
        float _radius;
        Drawable _orgDrawable;
        Drawable _orgTextBackground;

        protected override void OnAttached()
        {
            _view = Container ?? Control;

            _border = new GradientDrawable();
            _orgDrawable = _view.Background;
            if (Control is FormsEditTextBase editText)
            {
                _orgTextBackground = editText.Background;
                // hide underline.
                editText.Background = null;
            }

            UpdateRadius();
            UpdateWidth();
            UpdateColor();
            UpdateBorder();
        }

        protected override void OnDetached()
        {
            if (!IsDisposed)
            {    // Check disposed
                if (Control is FormsEditTextBase editText)
                {
                    editText.Background = _orgTextBackground;
                }

                if (Element is Label label)
                {
                    (_view as FormsTextView).SetBackgroundColor(label.BackgroundColor.ToAndroid());
                }
                else if (Element is Image image)
                {
                    (_view as ImageView).SetBackgroundColor(image.BackgroundColor.ToAndroid());
                }
                else
                {
                    _view.Background = _orgDrawable;
                }

                _view.SetPadding(0, 0, 0, 0);
                _view.ClipToOutline = false;

                System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            }
            _border?.Dispose();
            _border = null;
            _orgDrawable = null;
            _orgTextBackground = null;
            _view = null;
            System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached completely");
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (!IsSupportedByApi)
                return;

            if (IsDisposed)
            {
                return;
            }

            if (args.PropertyName == Border.RadiusProperty.PropertyName)
            {
                UpdateRadius();
                UpdateBorder();
            }
            else if (args.PropertyName == Border.WidthProperty.PropertyName)
            {
                UpdateWidth();
                UpdateBorder();
            }
            else if (args.PropertyName == Border.ColorProperty.PropertyName)
            {
                UpdateColor();
                UpdateBorder();
            }
            else if (args.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                UpdateBackgroundColor();
            }
        }

        void UpdateRadius()
        {
            _radius = _view.Context.ToPixels(Border.GetRadius(Element));
        }

        void UpdateWidth()
        {
            _width = (int)_view.Context.ToPixels(Border.GetWidth(Element) ?? 0);
        }

        void UpdateColor()
        {
            _color = Border.GetColor(Element).ToAndroid();
        }

        void UpdateBorder()
        {
            _border.SetStroke(_width, _color);
            _border.SetCornerRadius(_radius);

            var formsBack = (Element as Xamarin.Forms.View).BackgroundColor;
            if (formsBack != Xamarin.Forms.Color.Default)
            {
                _border.SetColor(formsBack.ToAndroid());
            }

            if (Element is BoxView)
            {
                var foreColor = ((BoxView)Element).Color;
                if (foreColor != Xamarin.Forms.Color.Default)
                {
                    _border.SetColor(foreColor.ToAndroid());
                }
            }

            _view.SetPadding(_width, _width, _width, _width);
            _view.ClipToOutline = true; //not to overflow children

            _view.SetBackground(_border);
        }

        void UpdateBackgroundColor()
        {
            _orgDrawable = _view.Background;
            UpdateBorder();
        }
    }

    [global::Android.Runtime.Preserve(AllMembers = true)]
    public abstract class EffectBase : PlatformEffect
    {
        public static bool IsFastRenderers = global::Xamarin.Forms.Forms.Flags.Any(x => x == "FastRenderers_Experimental");

        IVisualElementRenderer _renderer;
        bool _isDisposed = false;

        protected bool IsDisposed
        {
            get
            {
                if (_isDisposed)
                {
                    return _isDisposed;
                }

                if (_renderer == null)
                {
                    _renderer = (Container ?? Control) as IVisualElementRenderer;
                }

                if (IsFastRendererButton)
                {
                    return CheckButtonIsDisposed();
                }

                return _isDisposed = _renderer?.Tracker == null; //disposed check
            }
        }

        protected bool IsNullOrDisposed
        {
            get
            {
                if (Element.BindingContext == null)
                {
                    return true;
                }

                return IsDisposed;
            }
        }

        protected bool IsSupportedByApi => global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop;

        protected override void OnAttached()
        {
            if (!IsSupportedByApi)
                return;

            var visual = Element as VisualElement;
            var page = visual.Navigation.NavigationStack.LastOrDefault();
            if (page == null)
            {
                page = visual.Navigation.ModalStack.LastOrDefault();
                if (page == null)
                {
                    // In case the element in DataTemplate, NavigationProxycan't be got.
                    // Instead of it, the page dismissal is judged by whether the BindingContext is null.
                    Element.BindingContextChanged += BindingContextChanged;
                    OnAttachedOverride();
                    return;
                }
            }

            // To call certainly a OnDetached method when the page is popped, 
            // it executes the process removing all the effects in the page at once with Attached bindable property.
            if (!GetIsRegistered(page))
            {
                SetIsRegistered(page, true);
            }

            OnAttachedOverride();
        }

        protected virtual void OnAttachedOverride() { }

        protected override void OnDetached()
        {
            if (!IsSupportedByApi)
                return;

            OnDetachedOverride();

            System.Diagnostics.Debug.WriteLine($"Detached {GetType().Name} from {Element.GetType().FullName}");
            Element.BindingContextChanged -= BindingContextChanged;

            _renderer = null;
        }

        protected virtual void OnDetachedOverride() { }


        // whether Element is FastRenderer.(Exept Button)
        protected bool IsFastRenderer
        {
            get
            {
                //If Container is null, it regards this as FastRenderer Element.
                //But this judging may not become right in the future. 
                return IsFastRenderers && (Container == null && !(Element is Xamarin.Forms.Button));
            }
        }

        // whether Element is a Button of FastRenderer.
        protected bool IsFastRendererButton
        {
            get
            {
                return (IsFastRenderers && (Element is Xamarin.Forms.Button));
            }
        }

        // whether Element can add ClickListener.
        protected bool IsClickable
        {
            get
            {
                return !(IsFastRenderer || Element is Xamarin.Forms.Layout || Element is Xamarin.Forms.BoxView);
            }
        }

        static Func<object, object> GetDisposed; //cache

        // In case Button of FastRenderer, IVisualElementRenderer.Tracker don't become null.
        // So refered to the private field of "_disposed", judge whether being disposed. 
        bool CheckButtonIsDisposed()
        {
            if (GetDisposed == null)
            {
                GetDisposed = CreateGetField(typeof(VisualElementTracker));
            }
            _isDisposed = (bool)GetDisposed(_renderer.Tracker);

            return _isDisposed;
        }

        Func<object, object> CreateGetField(Type t)
        {
            var prop = t.GetRuntimeFields()
                .Where(x => x.DeclaringType == t && x.Name == "_disposed").FirstOrDefault();

            var target = Expression.Parameter(typeof(object), "target");

            var body = Expression.PropertyOrField(Expression.Convert(target, t), prop.Name);

            var lambda = Expression.Lambda<Func<object, object>>(
                Expression.Convert(body, typeof(object)), target
            );

            return lambda.Compile();
        }

        void BindingContextChanged(object sender, EventArgs e)
        {
            if (Element.BindingContext != null)
                return;

            // For Android, when a page is popped, OnDetached is automatically not called. (when iOS, it is called)
            // So, made the BindingContextChanged event subscribe in advance 
            // and make the effect manually removed when the BindingContext is null.
            // However, there is the problem that it isn't called when the BindingContext remains null all along.
            // The above solution is to use NavigationPage.Popped or Application.ModalPopping event.
            // That's why the following code runs only when the element is in a DataTemplate.
            if (IsAttached)
            {
                var toRemove = Element.Effects.OfType<AiRoutingEffectBase>().FirstOrDefault(x => x.EffectId == ResolveId);
                Device.BeginInvokeOnMainThread(() => Element?.Effects.Remove(toRemove));
            }
        }

        internal static readonly BindableProperty IsRegisteredProperty =
            BindableProperty.CreateAttached(
                    "IsRegistered",
                    typeof(bool),
                    typeof(EffectBase),
                    default(bool),
                    propertyChanged: IsRegisteredPropertyChanged
                );

        internal static void SetIsRegistered(BindableObject view, bool value)
        {
            view.SetValue(IsRegisteredProperty, value);
        }

        internal static bool GetIsRegistered(BindableObject view)
        {
            return (bool)view.GetValue(IsRegisteredProperty);
        }

        static void IsRegisteredPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bool)newValue) return;

            var page = bindable as Page;

            if (page.Parent is NavigationPage navi)
            {
                navi.Popped += NaviPopped;
            }
            else
            {
                Xamarin.Forms.Application.Current.ModalPopping += ModalPopping;
            }

            void NaviPopped(object sender, NavigationEventArgs e)
            {
                if (e.Page != page)
                    return;

                navi.Popped -= NaviPopped;

                RemoveEffects();
            }

            void ModalPopping(object sender, ModalPoppingEventArgs e)
            {
                if (e.Modal != page)
                    return;

                Xamarin.Forms.Application.Current.ModalPopping -= ModalPopping;

                RemoveEffects();
            }

            void RemoveEffects()
            {
                foreach (var child in page.Descendants())
                {
                    foreach (var effect in child.Effects.OfType<AiRoutingEffectBase>())
                    {
                        Device.BeginInvokeOnMainThread(() => child.Effects.Remove(effect));
                    }
                }

                foreach (var effect in page.Effects.OfType<AiRoutingEffectBase>())
                {
                    Device.BeginInvokeOnMainThread(() => page.Effects.Remove(effect));
                }

                page.ClearValue(IsRegisteredProperty);
            }
        }
    }
}
