using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Amlakbashi.Host.TagHelpers
{
    [HtmlTargetElement(Attributes = "is-disabled")]
    public class IsDisabledTagHelper : TagHelper
    {
        public bool IsDisabled { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperAttribute attr;
            if (output.Attributes.TryGetAttribute("disabled", out attr))
            {
                output.Attributes.Remove(attr);
            }
            if (IsDisabled)
            {
                output.Attributes.SetAttribute("disabled", "disabled");
            }
        }
    }
}
