using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Amlakbashi.Host.TagHelpers
{
    [HtmlTargetElement(Attributes = "is-checked")]
    public class IsCheckedTagHelper : TagHelper
    {
        public bool IsChecked { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("checked", IsChecked ? "checked" : "");
        }
    }
}
