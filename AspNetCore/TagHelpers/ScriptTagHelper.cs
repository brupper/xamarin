using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Brupper.AspNetCore.TagHelpers
{
    /*
    <script type="text/javascript" on-content-loaded="true">
        $('.button-collapse').sideNav();
    </script>
    */
    /// <summary> https://stackoverflow.com/questions/44511334/javascript-in-a-view-component </summary>
    [HtmlTargetElement("script", Attributes = "on-content-loaded")]
    public class ScriptTagHelper : TagHelper
    {
        /// <summary> Execute script only once document is loaded. </summary>
        public bool OnContentLoaded { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!OnContentLoaded)
            {
                base.Process(context, output);
            }
            else
            {
                var content = output.GetChildContentAsync().Result;
                var javascript = content.GetContent();

                var sb = new StringBuilder();

                sb.Append(@"/**
*  @section Scripts {} in viewcomponents is ignored and not rendered by Asp.Net rendering engine. So just use at the end of the view component. Also if your jquery scripts are at specified at the end in your layout, then jquery will not be available in your viewcomponents. Of course moving the jquery script to the head section in layout will solve the problem but it is recommended to load the js files at the end.
*    So if you want to keep jquery scripts at the end of layout and still use jquery in viewcomponents, you could use javascript domcontentloaded and any jquery can be written inside domcontentloaded. Not a permanent good approach but works for me.
 */
(function(setup) {
    if (/complete|interactive|loaded/.test(document.readyState)) {
        // In case the document has finished parsing, document's readyState will
        // be one of ""complete"", ""interactive"" or (non-standard) ""loaded"".
        setup();
    } else {
        document.addEventListener('DOMContentLoaded', function (event) {
            // The document is not ready yet, so wait for the DOMContentLoaded event
            setup();
        }, false);
    }
})(");
                sb.Append("function() {");
                sb.Append(javascript);
                sb.Append("});");

                output.Content.SetHtmlContent(sb.ToString());
            }
        }
    }
}
