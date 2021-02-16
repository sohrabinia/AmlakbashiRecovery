using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Amlakbashi.Host.TagHelpers
{
    [HtmlTargetElement("option", Attributes = "is-selected")]
    public class IsSelectedTagHelper : TagHelper
    {
        public bool IsSelected { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("selected", IsSelected ? "selected" : "");
        }
    }
}
