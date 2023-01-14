using System;
using System.Linq;
using System.Linq.Expressions;
using Xamarin.Forms;

// https://github.com/muak/AiForms.Effects
namespace Brupper.Forms.Effects
{
    /// <summary>
    /// Border.
    /// </summary>
    public static class Border
    {
        /// <summary>
        /// The on property.
        /// </summary>
        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                propertyName: "On",
                returnType: typeof(bool?),
                declaringType: typeof(Border),
                defaultValue: null,
                propertyChanged: AiRoutingEffectBase.ToggleEffectHandler<BorderRoutingEffect>
            );

        /// <summary>
        /// Sets the on.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="value">Value.</param>
        public static void SetOn(BindableObject view, bool? value)
        {
            view.SetValue(OnProperty, value);
        }

        /// <summary>
        /// Gets the on.
        /// </summary>
        /// <returns>The on.</returns>
        /// <param name="view">View.</param>
        public static bool? GetOn(BindableObject view)
        {
            return (bool?)view.GetValue(OnProperty);
        }

        /// <summary>
        /// The radius property.
        /// </summary>
        public static readonly BindableProperty RadiusProperty =
            BindableProperty.CreateAttached(
                "Radius",
                typeof(double),
                typeof(Border),
                default(double),
                propertyChanged: AiRoutingEffectBase.AddEffectHandler<BorderRoutingEffect>
            );

        /// <summary>
        /// Sets the radius.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="value">Value.</param>
        public static void SetRadius(BindableObject view, double value)
        {
            view.SetValue(RadiusProperty, value);
        }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <returns>The radius.</returns>
        /// <param name="view">View.</param>
        public static double GetRadius(BindableObject view)
        {
            return (double)view.GetValue(RadiusProperty);
        }

        /// <summary>
        /// The width property.
        /// </summary>
        public static readonly BindableProperty WidthProperty =
            BindableProperty.CreateAttached(
                "Width",
                typeof(double?),
                typeof(Border),
                default(double?),
                propertyChanged: AiRoutingEffectBase.AddEffectHandler<BorderRoutingEffect>
            );

        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="value">Value.</param>
        public static void SetWidth(BindableObject view, double? value)
        {
            view.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <returns>The width.</returns>
        /// <param name="view">View.</param>
        public static double? GetWidth(BindableObject view)
        {
            return (double?)view.GetValue(WidthProperty);
        }

        /// <summary>
        /// The color property.
        /// </summary>
        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(Border),
                Color.Transparent
            );

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="value">Value.</param>
        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="view">View.</param>
        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

    }

    internal class BorderRoutingEffect : AiRoutingEffectBase
    {
        public BorderRoutingEffect() : base("BrupperX." + nameof(Border)) { }
    }

    public class AiRoutingEffectBase : RoutingEffect
    {
        internal static class InstanceCreator<TInstance>
        {
            public static Func<TInstance> Create { get; } = CreateInstance();

            static Func<TInstance> CreateInstance()
            {
                return Expression.Lambda<Func<TInstance>>(Expression.New(typeof(TInstance))).Compile();
            }
        }

        public static void AddEffectHandler<TRoutingEffect>(BindableObject bindable, object oldValue, object newValue) where TRoutingEffect : AiRoutingEffectBase
        {
            //if (!EffectConfig.EnableTriggerProperty)
            //    return;

            //if (!(bindable is VisualElement element) || newValue == null)
            //    return;

            //if (EffectShared<TRoutingEffect>.GetIsTriggered(element))
            //    return;

            //AddEffect<TRoutingEffect>(element);
        }

        public static void ToggleEffectHandler<TRoutingEffect>(BindableObject bindable, object oldValue, object newValue) where TRoutingEffect : AiRoutingEffectBase
        {
            if (!(bindable is VisualElement element)) return;

            var enabled = (bool?)newValue;

            if (!enabled.HasValue) return;

            if (enabled.Value)
            {
                AddEffect<TRoutingEffect>(element);
            }
            else
            {
                RemoveEffect<TRoutingEffect>(element);
            }
        }

        static void AddEffect<T>(Element element) where T : AiRoutingEffectBase
        {
            if (!element.Effects.OfType<T>().Any())
            {
                element.Effects.Add(InstanceCreator<T>.Create());
                EffectShared<T>.SetIsTriggered(element, true);
            }
        }

        static void RemoveEffect<T>(Element element) where T : AiRoutingEffectBase
        {
            var remove = element.Effects.OfType<T>().FirstOrDefault();
            if (remove != null)
            {
                element.Effects.Remove(remove);
                // to avoid duplicate trigger
                Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                {
                    element.ClearValue(EffectShared<T>.IsTriggeredProperty);
                    return false;
                });
            }
        }


        public string EffectId { get; }
        public AiRoutingEffectBase(string effectId) : base(effectId)
        {
            EffectId = effectId;
        }
    }

    public static class EffectShared<T> where T : AiRoutingEffectBase
    {
        public static readonly BindableProperty IsTriggeredProperty =
            BindableProperty.CreateAttached(
                    "IsTriggered",
                    typeof(bool),
                    typeof(EffectShared<T>),
                    default(bool)
                );

        public static void SetIsTriggered(BindableObject view, bool value)
        {
            view.SetValue(IsTriggeredProperty, value);
        }

        public static bool GetIsTriggered(BindableObject view)
        {
            return (bool)view.GetValue(IsTriggeredProperty);
        }
    }
}
