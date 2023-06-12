using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace InspEx.Data.Localization.ResxLocalization
{
    public class ResxTextProvider : ITextProvider
    {
        protected readonly IList<ResourceManager> resourceManagers;

        #region Constructors

        public ResxTextProvider(IList<ResourceManager> resourceManagers)
        {
            this.resourceManagers = resourceManagers;
            CurrentLanguage = CultureInfo.CurrentUICulture;
        }

        public ResxTextProvider(ResourceManager resourceManager)
            : this(new List<ResourceManager>() { resourceManager })
        { }

        #endregion

        public CultureInfo CurrentLanguage { get; set; }

        public virtual string GetText(string namespaceKey, string typeKey, string name)
        {
            var resolvedKey = name;

            if (!string.IsNullOrEmpty(typeKey))
            {
                resolvedKey = $"{typeKey}.{resolvedKey}";
            }

            if (!string.IsNullOrEmpty(namespaceKey))
            {
                resolvedKey = $"{namespaceKey}.{resolvedKey}";
            }

            foreach (var resourceManager in resourceManagers)
            {
                var text = resourceManager.GetString(resolvedKey, CurrentLanguage);
                if (!string.IsNullOrEmpty(text))
                    return text;
            }
            return null;
        }

        public virtual string GetText(string namespaceKey, string typeKey, string name, params object[] formatArgs)
        {
            var baseText = GetText(namespaceKey, typeKey, name);

            if (string.IsNullOrEmpty(baseText))
            {
                return baseText;
            }

            return string.Format(baseText, formatArgs);
        }

        public virtual bool TryGetText(out string textValue, string namespaceKey, string typeKey, string name)
        {
            textValue = GetText(namespaceKey, typeKey, name);
            return textValue != null;
        }

        public virtual bool TryGetText(out string textValue, string namespaceKey, string typeKey, string name, params object[] formatArgs)
        {
            if (!TryGetText(out textValue, namespaceKey, typeKey, name))
                return false;

            // Key is found but matching value is empty. Don't format but return true.
            if (string.IsNullOrEmpty(textValue))
                return true;

            textValue = string.Format(textValue, formatArgs);
            return true;
        }
    }

}