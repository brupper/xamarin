using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace Brupper.Forms.Converters
{
    public class HtmlLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var formatted = new FormattedString();

            if (value is string stringValue)
                foreach (var item in ProcessString(stringValue))
                    formatted.Spans.Add(CreateSpan(item));

            return formatted;
        }

        private Span CreateSpan(StringSection section)
        {
            var span = new Span()
            {
                Text = section.Text
            };

            if (!string.IsNullOrEmpty(section.Link))
            {
                span.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = _navigationCommand,
                    CommandParameter = section.Link
                });
                span.TextColor = Color.Blue;
                span.TextDecorations = TextDecorations.Underline;
            }

            return span;
        }

        public static IList<StringSection> ProcessString(string rawText)
        {
            const string spanPattern = @"(<a.*?>.*?</a>)";

            var collection = Regex.Matches(rawText, spanPattern, RegexOptions.Singleline);

            var sections = new List<StringSection>();

            var lastIndex = 0;

            foreach (Match item in collection)
            {
                try
                {
                    var foundText = item.Value;
                    var text = rawText?.Substring(lastIndex, item.Index - lastIndex) ?? string.Empty;
                    sections.Add(new StringSection { Text = text });
                    lastIndex += ((item.Index - lastIndex) + item.Length);

                    // Get HTML href 
                    var html = new StringSection()
                    {
                        Link = Regex.Match(item.Value, "(?<=href=\\\")[\\S]+(?=\\\")").Value,
                        Text = Regex.Replace(item.Value, "<.*?>", string.Empty)
                    };

                    sections.Add(html);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }

            try
            {
                sections.Add(new StringSection() { Text = rawText?.Substring(lastIndex) });
            }
            catch { /* ignore */ }

            return sections;
        }

        public class StringSection
        {
            public string Text { get; set; }
            public string Link { get; set; }
        }

        private ICommand _navigationCommand = new Command<string>((url) =>
        {
            _ = Xamarin.Essentials.Browser.OpenAsync(new Uri(url));
        });

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
